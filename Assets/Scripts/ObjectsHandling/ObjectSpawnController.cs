using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectSpawnController : MonoBehaviour
{
    [SerializeField] private GameObject transformModeUi;
    [SerializeField] private Button approveButton;
    [SerializeField] private Button revertButton;
    [SerializeField] private ObjectsTransformController objectsTransformController;

    private ARRaycastManager _arRaycastManager;

    private GameObject _spawnedObject;

    private void Start()
    {
        NullComponentChecker.LogIfComponentNull(approveButton,
            $"Approve Button must be defined in {nameof(ObjectSpawnController)}");

        transformModeUi.gameObject.SetActive(false);
        approveButton.onClick.AddListener(() =>
        {
            objectsTransformController.RemoveChild(_spawnedObject.transform);
            _spawnedObject = null;
        });
        revertButton.onClick.AddListener(() =>
        {
            Destroy(_spawnedObject);
            _spawnedObject = null;
        });

        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        NullComponentChecker.LogIfComponentNull(_arRaycastManager,
            $"{nameof(ARRaycastManager)} must be created to use {nameof(ObjectSpawnController)}");
    }

    public void SpawnObject(GameObject prefab)
    {
        if (!Camera.main) return;
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        _arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        var thisTransform = transform;
        var pose = hits.Count > 0
            ? hits[0].pose
            : new Pose(thisTransform.position, thisTransform.rotation);

        _spawnedObject = Instantiate(prefab, pose.position, pose.rotation);

        transformModeUi.gameObject.SetActive(true);
    }

    private GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        var obj = Object.Instantiate(prefab, position, rotation);
        objectsTransformController.AddChild(obj.transform);
        return obj;
    }
}
