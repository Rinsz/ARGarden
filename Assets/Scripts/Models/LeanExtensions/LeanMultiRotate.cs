using System.Collections.Generic;
using CW.Common;
using Lean.Touch;
using UnityEngine;

namespace Models
{
    public class LeanMultiRotate : MonoBehaviour
    {
        private Camera camera;
        [SerializeField] private float sensitivity = 10f;

        public LeanFingerFilter Use = new(true);

        protected virtual void Awake()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }

        protected virtual void Update()
        {
            var fingers = Use.UpdateAndGetFingers();
            camera = CwHelper.GetCamera(camera, gameObject);
            if (!camera)
            {
                Debug.LogError(
                    "Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);

                return;
            }

            if (fingers.Count == 1)
                RotateXY(fingers);
            else
                RotateZ(fingers);
        }

        private void RotateXY(List<LeanFinger> fingers)
        {
            var screenDelta = LeanGesture.GetScreenDelta(fingers);
            if (screenDelta == Vector2.zero)
                return;

            var cameraTransform = camera.transform;
            var rotateVector = cameraTransform.right * screenDelta.y - cameraTransform.up * screenDelta.x;
            transform.Rotate(rotateVector, sensitivity, Space.World);
        }

        private void RotateZ(List<LeanFinger> fingers)
        {
            var twistDegrees = LeanGesture.GetTwistDegrees(fingers);

            var cameraForward = camera.transform.forward;
            transform.Rotate(cameraForward, sensitivity * twistDegrees, Space.World);
        }
    }
}