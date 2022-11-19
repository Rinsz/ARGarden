using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Touch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VrObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button approveButton;

    private ARRaycastManager _arRaycastManager;

    private GameObject _spawnedObject;

    private void Start()
    {
        if (buttons.Length != objectsToSpawn.Length)
            throw new Exception("objects to spawn count isn't equal to buttons count");

        for (var i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[index].onClick.AddListener(() =>
            {
                foreach (var button in buttons)
                    button.gameObject.SetActive(false);
                PlaceObject(objectsToSpawn[index]);
            });
        }

        approveButton.gameObject.SetActive(false);
        approveButton.onClick.AddListener(() =>
        {
            approveButton.gameObject.SetActive(false);
            foreach (var button in buttons)
                button.gameObject.SetActive(true);
            _spawnedObject.GetComponent<LeanDragTranslate>().enabled = false;
            _spawnedObject.GetComponent<LeanPinchScale>().enabled = false;
            _spawnedObject.GetComponent<LeanTwistRotateAxis>().enabled = false;
            _spawnedObject = null;
        });
        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void PlaceObject(GameObject prefab)
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        _arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        var thisTransform = transform;
        var pose = hits.Count > 0
            ? hits[0].pose
            : new Pose(thisTransform.position, thisTransform.rotation);

        _spawnedObject = Instantiate(prefab, pose.position, pose.rotation);
        _spawnedObject.AddComponent<LeanDragTranslate>();
        _spawnedObject.AddComponent<LeanPinchScale>();
        _spawnedObject.AddComponent<LeanTwistRotateAxis>();

        approveButton.gameObject.SetActive(true);
    }
}
