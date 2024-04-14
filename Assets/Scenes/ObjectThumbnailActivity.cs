using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThumbGenExamples
{
    public class ObjectThumbnailActivity
    {
        private GameObject _object;
        private ThumbnailGenerator _thumbGen;

        private string _resourceName;

        public ObjectThumbnailActivity(ThumbnailGenerator thumbGen, string resourceName)
        {
            _thumbGen = thumbGen;
            _resourceName = resourceName;
        }

        public bool Setup()
        {
            if (string.IsNullOrWhiteSpace(_resourceName))
                return false;

            Debug.Log("Instantiating: " + _resourceName);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_resourceName);

            GameObject stage = GameObject.Find("Stage");

            _object = GameObject.Instantiate(prefab, stage?.transform);

            Camera cam = _thumbGen.ThumbnailCamera.GetComponent<Camera>();

            MeshResultData meshResultData = FindMeshContainerInChildren(_object);
            if (meshResultData == null)
            {
                Debug.LogWarning("No mesh found!");
                return false;
            }
            MoveCamera(cam, meshResultData.GameObject);

            //cam.transform.LookAt(stage.transform);

            return true;
        }

        private void MoveCamera(Camera cam, GameObject go)
        {
            Bounds bounds = go.GetComponent<Renderer>().bounds;
            float radius = bounds.extents.magnitude;
            //Gizmos.DrawWireSphere(bounds.center, radius * 2);
            cam.transform.position = RaySphereIntersectionPoint(new Ray(bounds.center, new Vector3(-2.5f, -1f, -2.5f)), bounds.center, radius * 2);
            cam.transform.LookAt(bounds.center);
        }

        public Vector3 RaySphereIntersectionPoint(Ray ray, Vector3 sphereCenter, float sphereRadius)
        {
            // Compute the direction from the ray origin to the sphere center
            Vector3 rayToSphereCenter = sphereCenter - ray.origin;

            // Compute the projection of the ray direction onto the direction from the ray origin to the sphere center
            float tProjection = Vector3.Dot(ray.direction, rayToSphereCenter);

            // Compute the distance between the ray origin and the point of closest approach to the sphere center
            float distanceToClosestApproach = Vector3.Distance(ray.origin, ray.origin + ray.direction * tProjection);

            // Compute the distance from the closest approach to the intersection point
            float distanceToIntersection = Mathf.Sqrt(sphereRadius * sphereRadius - distanceToClosestApproach * distanceToClosestApproach);

            // Compute the intersection point
            Vector3 intersectionPoint = ray.origin + ray.direction * (tProjection - distanceToIntersection);

            return intersectionPoint;
        }

        /*
         * Alternative way to position the camera
         * 
        private void MoveCamera(Camera cam, Bounds bounds)
        {
            float cameraDistance = 0.5f; // Constant factor
            Vector3 objectSizes = bounds.max - bounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
            cam.transform.position = bounds.center + distance * Vector3.one;
        }
        */

        public MeshResultData FindMeshContainerInChildren(GameObject go)
        {
            if (!go.activeInHierarchy)
            {
                return null;
            }

            Mesh mesh = GetMesh(go);
            if (mesh != null)
            {
                return new MeshResultData(mesh, go);
            }

            foreach (Transform child in go.transform)
            {
                MeshResultData childMeshResult = FindMeshContainerInChildren(child.gameObject);
                if (childMeshResult != null)
                {
                    return childMeshResult;
                }
            }

            return null;
        }

        private Mesh GetMesh(GameObject go)
        {
            try
            {
                return go.GetComponent<MeshCollider>().sharedMesh;
            }
            catch (MissingComponentException)
            {
                try
                {
                    return go.GetComponent<MeshFilter>().mesh;
                }
                catch (MissingComponentException)
                {
                    try
                    {

                        return go.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    }
                    catch (MissingComponentException)
                    {
                        return null;
                    }
                }
            }
        }

        public bool CanProcess()
        {
            return true;
        }

        public void Process()
        {
            if (_thumbGen == null)
                return;

            string[] splitName = _resourceName.Split('/');
            string name = splitName[splitName.Length - 1].Split('.')[0];
            //Debug.Log("Processing: " + name);
            _thumbGen.Render(name);
        }

        public void Cleanup()
        {
            if (_object != null)
            {
                GameObject.Destroy(_object);

                _object = null;
            }
        }
    }
}