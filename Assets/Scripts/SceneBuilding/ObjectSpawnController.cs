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
    [SerializeField] private Color handledObjectColor = Color.white;

    public UnityEvent<TranslatedObject> OnSpawned = new();

    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arRaycastManager.LogIfComponentNull(
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

        var translatedObject = obj.AddComponent<TranslatedObject>();
        var outline = obj.GetComponent<Outline>();
        outline.OutlineColor = handledObjectColor;
        outline.OutlineWidth = 5;
        OnSpawned.Invoke(translatedObject);
    }
}
