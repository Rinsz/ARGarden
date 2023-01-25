using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class HDRLightEstimation : MonoBehaviour
{
    private Light _light;
    private ARCameraManager _cameraManager;

    private void Start()
    {
        _light = GetComponent<Light>();
        _cameraManager = FindObjectOfType<ARCameraManager>();

        if (_cameraManager != null)
        {
            _cameraManager.frameReceived += OnFrameReceived;
        }
    }

    private void OnDestroy()
    {
        if (_cameraManager != null)
        {
            _cameraManager.frameReceived -= OnFrameReceived;
        }
    }

    private void OnFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // Light intensity:
        if (eventArgs.lightEstimation.mainLightIntensityLumens.HasValue)
        {
            _light.intensity = eventArgs.lightEstimation.mainLightIntensityLumens.Value;
        }
        else if (eventArgs.lightEstimation.averageBrightness.HasValue)
        {
            _light.intensity = eventArgs.lightEstimation.averageBrightness.Value;
        }

        // Light color:
        if (eventArgs.lightEstimation.mainLightColor.HasValue)
        {
            _light.color = eventArgs.lightEstimation.mainLightColor.Value;
        }
        else if (eventArgs.lightEstimation.colorCorrection.HasValue)
        {
            _light.color = eventArgs.lightEstimation.colorCorrection.Value;
        }

        // Color Temperature:
        if (eventArgs.lightEstimation.averageColorTemperature.HasValue)
        {
            _light.colorTemperature = eventArgs.lightEstimation.averageColorTemperature.Value;
        }

        // Light direction:
        if (eventArgs.lightEstimation.mainLightDirection.HasValue)
        {
            _light.transform.rotation = Quaternion.LookRotation(
                eventArgs.lightEstimation.mainLightDirection.Value);
        }

        // Ambinent Probe:
        if (eventArgs.lightEstimation.ambientSphericalHarmonics.HasValue)
        {
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = eventArgs.lightEstimation.ambientSphericalHarmonics.Value;
        }
    }
}
