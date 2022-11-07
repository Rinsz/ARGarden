using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Touch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacementController : MonoBehaviour
{
    public GameObject placementIndicator;
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button approveButton;
    private GameObject _arObjectToSpawn;
    private GameObject _spawnedObject;
    private Pose _placementPose;
    private ARRaycastManager _arRaycastManager;
    private bool _placementPoseIsValid;
    private ObjectPlacementControllerState _state = ObjectPlacementControllerState.ChoosingObject;

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
                _arObjectToSpawn = objectsToSpawn[index];
                _state = ObjectPlacementControllerState.PlacingObject;
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
            _state = ObjectPlacementControllerState.ChoosingObject;
        });
        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    private void Update()
    {
        if (_state == ObjectPlacementControllerState.PlacingObject)
            TryPlaceObject();
    }

    private void TryPlaceObject()
    {
        if(_spawnedObject == null
           && _placementPoseIsValid
           && Input.touchCount > 0
           && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }

        UpdatePlacementIndicator();
    }

    private void UpdatePlacementIndicator()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        _arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        _placementPoseIsValid = hits.Count > 0;
        if(_placementPoseIsValid)
        {
            _placementPose = hits[0].pose;
        }
        if(_spawnedObject == null && _placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void ARPlaceObject()
    {
        _spawnedObject = Instantiate(_arObjectToSpawn, _placementPose.position, _placementPose.rotation);
        approveButton.gameObject.SetActive(true);
        _state = ObjectPlacementControllerState.TranslatingObject;
    }
}
