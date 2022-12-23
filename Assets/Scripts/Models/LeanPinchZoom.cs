
using CW.Common;
using Lean.Touch;
using UnityEngine;

namespace Models
{
    public class LeanPinchZoom : MonoBehaviour
    {
	    private Camera сamera;
	    private LeanFingerFilter Use = new(true);

		public float Sensitivity = 0.7f;

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			var fingers = Use.UpdateAndGetFingers();

			var pinchScale = LeanGesture.GetPinchScale(fingers);
			var pinchFingersCenter = LeanGesture.GetScreenCenter(fingers);

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
				Debug.LogError("Failed to find camera. Either tag your camera as MainCamera, or set one in this component.", this);
			}
		}
    }
}