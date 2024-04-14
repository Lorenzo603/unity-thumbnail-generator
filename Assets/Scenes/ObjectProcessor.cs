using System.Collections;
using ThumbGenExamples;
using UnityEngine;

public class ObjectProcessor : MonoBehaviour
{
    private ObjectThumbnailActivity _activity;

    void Start()
    {
    }
    void Awake()
    {
        ThumbnailGenerator thumbGen = GetComponent<ThumbnailGenerator>();


        _activity = new ObjectThumbnailActivity(thumbGen, "Assets/PolygonAncientEmpire/Prefabs/Environment/SM_Env_Background_Islands_01.prefab");
        StartCoroutine(DoProcess());
    }

    IEnumerator DoProcess()
    {
        yield return new WaitForEndOfFrame();

        _activity.Setup();

        if (!_activity.CanProcess())

            yield return null;

        _activity.Process();

        StartCoroutine(DoCleanup());
    }

    IEnumerator DoCleanup()
    {
        yield return new WaitForEndOfFrame();

        _activity.Cleanup();
    }
}