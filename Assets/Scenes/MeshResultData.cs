using UnityEngine;

namespace ThumbGenExamples
{
    public class MeshResultData
    {
        public Mesh Mesh { get; private set; }
        public GameObject GameObject { get; private set; }

        public MeshResultData(Mesh mesh, GameObject go)
        {
            Mesh = mesh;
            GameObject = go;
        }
    }
}