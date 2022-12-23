using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ObjectEditController : MonoBehaviour
{
    [SerializeField] private GameObject transformModeUi;
    [SerializeField] private Button approveButton;
    [SerializeField] private Button revertButton;
    [SerializeField] private ObjectsTransformController objectsTransformController;

    private ARRaycastManager _arRaycastManager;

    private Dictionary<Transform, Vector3> objectsToOldPositions = new();

    private void Start()
    {
        NullComponentChecker.LogIfComponentNull(approveButton,
            $"Approve Button must be defined in {nameof(ObjectEditController)}");

        transformModeUi.gameObject.SetActive(false);
        approveButton.onClick.AddListener(() =>
        {
            foreach (var kv in objectsToOldPositions)
            {
                objectsTransformController.RemoveChild(kv.Key);
            }
            objectsToOldPositions.Clear();
        });
        revertButton.onClick.AddListener(() =>
        {
            foreach (var kv in objectsToOldPositions)
            {
                objectsTransformController.RemoveChild(kv.Key);
                kv.Key.position = kv.Value;
            }
            objectsToOldPositions.Clear();
        });

        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        NullComponentChecker.LogIfComponentNull(_arRaycastManager,
            $"{nameof(ARRaycastManager)} must be created to use {nameof(ObjectEditController)}");
    }

    public void Update()
    {
        var cam = Camera.main;
        if (!cam) return;
        var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        Physics.Raycast(screenCenter, Vector3.forward, out var hit);

        if (!hit.transform) return;

        if (objectsTransformController.ContainsChild(hit.transform))
        {
            objectsTransformController.AddChild(hit.transform);
            objectsToOldPositions.Add(hit.transform, hit.transform.position);
        }
        else
        {
            objectsTransformController.RemoveChild(hit.transform);
            objectsToOldPositions.Remove(hit.transform);
        }

        transformModeUi.gameObject.SetActive(true);
    }
}