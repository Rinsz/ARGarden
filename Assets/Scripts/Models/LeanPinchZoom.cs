using CW.Common;
using Lean.Touch;
using UnityEngine;

namespace Models
{
    public class LeanPinchZoom : MonoBehaviour
    {
        public float Sensitivity = 0.7f;
        private Camera сamera;
        private readonly LeanFingerFilter Use = new(true);

        protected virtual void Awake()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }

        protected virtual void Update()
        {
            var fingers = Use.UpdateAndGetFingers();

            var pinchScale = LeanGesture.GetPinchScale(fingers);

            Translate(pinchScale);
        }

        private void Translate(float pinchScale)
        {
            var camera = CwHelper.GetCamera(сamera, gameObject);

            if (camera != null)
            {
                var positionFromScreen = camera.WorldToScreenPoint(transform.position);

                positionFromScreen += Vector3.back * ((pinchScale - 1) * Sensitivity * Screen.width * Time.deltaTime);

                transform.position = camera.ScreenToWorldPoint(positionFromScreen);
            }
            else
            {
                Debug.LogError(
                    "Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
            }
        }
    }
}