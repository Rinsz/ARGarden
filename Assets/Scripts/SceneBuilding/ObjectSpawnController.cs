using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectSpawnController : MonoBehaviour
{
    private Camera camera;
    private ARRaycastManager arRaycastManager;

    [SerializeField] private ObjectsTransformController objectsTransformController;

    public UnityEvent<GameObject> OnSpawned = new();

    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        NullComponentChecker.LogIfComponentNull(arRaycastManager,
            $"{nameof(ARRaycastManager)} must be created to use {nameof(ObjectSpawnController)}");
    }

    public void SpawnObject(GameObject prefab)
    {
        camera ??= Camera.main;
        if (!camera) return;
        var screenCenter = camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        var thisTransform = transform;
        var pose = hits.Count > 0
            ? hits[0].pose
            : new Pose(thisTransform.position, thisTransform.rotation);

        Instantiate(prefab, pose.position, pose.rotation);
    }

    private void Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var obj = Object.Instantiate(prefab, position, rotation);
        objectsTransformController.AddChild(obj.transform);

        obj.AddComponent<TranslatedObject>();
        var outline = obj.AddComponent<Outline>();
        outline.OutlineColor = new Color(117, 180, 236);
        outline.OutlineWidth = 5;
        OnSpawned.Invoke(obj);
    }
}
