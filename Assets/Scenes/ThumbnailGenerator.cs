using System.Collections;
using System.IO;
using UnityEngine;

namespace ThumbGenExamples
{
    public class ThumbnailGenerator : MonoBehaviour
    {
        public RenderTexture TargetRenderTexture;

        public Camera ThumbnailCamera;

        public int ThumbnailWidth;
        public int ThumbnailHeight;

        public Texture2D TextureResult { get; private set; }

        public string ExportFilePathRoot;

        void Start()
        {
            ThumbnailWidth = 512;
            ThumbnailHeight = 512;
        }

        private void AssignRenderTextureToCamera()
        {
            if (ThumbnailCamera != null && TargetRenderTexture != null)
            {
                ThumbnailCamera.targetTexture = TargetRenderTexture;
            }
            else if (ThumbnailCamera.targetTexture != null)
            {
                TargetRenderTexture = ThumbnailCamera.targetTexture;
            }
        }

        public void Render(string filename)
        {
            StartCoroutine(DoRender(filename));
        }

        IEnumerator DoRender(string filename)
        {
            yield return new WaitForEndOfFrame();

            ExecuteRender(filename);
        }

        private void ExecuteRender(string filename)
        {
            if (ThumbnailCamera == null)
            {
                throw new System.InvalidOperationException("ThumbnailCamera not found. Please assign one to the ThumbnailGenerator.");
            }

            if (TargetRenderTexture == null && ThumbnailCamera.targetTexture == null)
            {
                throw new System.InvalidOperationException("RenderTexture not found. Please assign one to the ThumbnailGenerator.");
            }

            AssignRenderTextureToCamera();

            Texture2D tex = null;

            {   // Create the texture from the RenderTexture

                RenderTexture.active = TargetRenderTexture;

                tex = new Texture2D(ThumbnailWidth, ThumbnailHeight);

                tex.ReadPixels(new Rect(0, 0, ThumbnailWidth, ThumbnailHeight), 0, 0);
                tex.Apply();

                TextureResult = tex;
            }

            // Export to the file system, if ExportFilePath is specified.
            if (tex != null && !string.IsNullOrWhiteSpace(ExportFilePathRoot) && !string.IsNullOrWhiteSpace(filename))
            {

                string name = getJustFileName(filename);
                //Debug.Log("filename: " + filename);
                //Debug.Log("name: " + name);
                string outputDir = Application.dataPath + ExportFilePathRoot + filename.Replace(name, "");
                if (!System.IO.Directory.Exists(outputDir))
                {
                    //Debug.Log("Creating dir");
                    System.IO.Directory.CreateDirectory(outputDir);
                }

                string finalPath = string.Format("{0}{1}{2}.png", outputDir, Path.DirectorySeparatorChar, name);

                byte[] bytes = tex.EncodeToPNG();
                System.IO.File.WriteAllBytes(finalPath, bytes);
            }
        }

        private string getJustFileName(string filename)
        {
            int lastFolderMarker = filename.LastIndexOf(Path.DirectorySeparatorChar);
            return lastFolderMarker == -1 ? filename : filename.Substring(lastFolderMarker);
        }
    }
}