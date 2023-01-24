using System;
using CW.Common;
using Lean.Touch;
using UnityEngine;

namespace Models
{
    public class LeanPinchZoom : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 0.7f;
        [SerializeField] private float minimalDistance = 1;

        private Camera camera;
        private readonly LeanFingerFilter Use = new(true);

        protected virtual void Awake()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }

        protected virtual void Update()
        {
            var fingers = Use.UpdateAndGetFingers();
            if (fingers.Count < 2) return;
            camera = CwHelper.GetCamera(camera, gameObject);
            if (!camera)
            {
                Debug.LogError(
                    "Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);

                return;
            }

            var pinchScale = LeanGesture.GetPinchScale(fingers);

            Translate(pinchScale, camera);
        }

        private void Translate(float pinchScale, Camera camera)
        {
            var positionFromScreen = camera.WorldToScreenPoint(transform.position);

            positionFromScreen += Vector3.back * ((pinchScale - 1) * sensitivity * Screen.width * Time.deltaTime);

            transform.position = GetBorderedPosition(camera.ScreenToWorldPoint(positionFromScreen), camera.transform);
        }

        private Vector3 GetBorderedPosition(Vector3 position, Transform cameraTransform)
        {
            var cameraPosition = cameraTransform.position;
            var cameraDirection = cameraTransform.forward;

            var borderStartPosition = cameraPosition + cameraDirection * minimalDistance;
            var positionFromBorderStart = position - borderStartPosition;

            var angleToForward = Vector3.Angle(positionFromBorderStart, cameraDirection) * (float)Math.PI / 180f;
            var cosToForward = Mathf.Cos(angleToForward);
            if (cosToForward >= 0)
                return position;

            var distanceFromBorderSurface = positionFromBorderStart.magnitude * cosToForward;

            return position - cameraDirection * distanceFromBorderSurface;
        }
    }
}