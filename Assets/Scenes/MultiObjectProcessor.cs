using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ThumbGenExamples
{
    public class MultiObjectProcessor : MonoBehaviour
    {
        private List<ObjectThumbnailActivity> _activities;

        private ObjectThumbnailActivity _curActivity;

        public string assetDir;
        public string excludedDirs;

        void Start()
        {
        }

        void Awake()
        {
            _curActivity = null;
            _activities = new List<ObjectThumbnailActivity>();

            ThumbnailGenerator thumbGen = GetComponent<ThumbnailGenerator>();

            /*
            string[] objectResourceNames =
            {
                "objects/Cube",
                "objects/Cylinder",
                "objects/Capsule",
                "objects/Sphere"
            };
            */
            /*
            DirectoryInfo dir = new DirectoryInfo(assetDir);
            FileInfo[] info = dir.GetFiles();
            string[] objectResourceNames = info
                .Where(f => f.Name.EndsWith(".prefab"))
                .Select(f => assetDir + "/" + f.Name).ToArray();
            */
            string projectFolder = Directory.GetParent(Application.dataPath).ToString() + Path.DirectorySeparatorChar;
            Debug.Log("Project Folder: " + projectFolder);
            List<string> objectResourceNames = TraverseFolder(assetDir, projectFolder, excludedDirs.Split(','));

            foreach (var name in objectResourceNames)
            {
                var thumbActivity = new ObjectThumbnailActivity(thumbGen, name);
                _activities.Add(thumbActivity);
            }
        }

        List<string> TraverseFolder(string folderPath, string projectFolder, string[] excludedFolders)
        {
            List<string> fileList = new List<string>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(folderPath);
                if (excludedFolders.Contains(dir.Name))
                {
                    return fileList;
                }

                FileInfo[] info = dir.GetFiles();
                string[] objectResourceNames = info
                    .Where(f => f.Name.EndsWith(".prefab"))
                    .Select(f => f.ToString().Replace(projectFolder, "")).ToArray();
                foreach (string file in objectResourceNames)
                {
                    fileList.Add(file);
                }

                // Get all subdirectories in the current folder
                string[] subfolders = Directory.GetDirectories(folderPath);
                foreach (string subfolder in subfolders)
                {
                    List<string> subfolderFiles = TraverseFolder(subfolder, projectFolder, excludedFolders);
                    fileList.AddRange(subfolderFiles);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error traversing folder: " + e.Message);
            }
            return fileList;
        }

        void Update()
        {
            if (_curActivity == null)
            {
                if (_activities.Count > 0)
                {
                    _curActivity = _activities[0];

                    _curActivity.Setup();

                    StartCoroutine("DoProcess");

                    _activities.RemoveAt(0);
                }
            }
        }

        IEnumerator DoProcess()
        {
            yield return new WaitForEndOfFrame();

            if (_curActivity == null)
                yield return null;

            if (!_curActivity.CanProcess())
                yield return null;

            _curActivity.Process();

            StartCoroutine(DoCleanup());
        }

        IEnumerator DoCleanup()
        {
            yield return new WaitForEndOfFrame();

            if (_curActivity != null)
            {
                _curActivity.Cleanup();
                _curActivity = null;
            }
        }
    }
}