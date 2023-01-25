using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

[UsedImplicitly]
public class MotionStereoDepthDataSource : IDepthDataSource
{
    private Texture2D _confidenceTexture = Texture2D.blackTexture;
    private Matrix4x4 _depthDisplayMatrix = Matrix4x4.identity;

    private short[] _depthArray = new short[0];
    private byte[] _depthBuffer = new byte[0];
    private byte[] _confidenceArray = new byte[0];

    private int _depthHeight = 0;
    private int _depthWidth = 0;
    private bool _initialized = false;
    private bool _useRawDepth = false;

    private XRCameraIntrinsics _depthCameraIntrinsics;
    private ARCameraManager _cameraManager;
    private AROcclusionManager _occlusionManager;
    private ARCameraBackground _cameraBackground;

    private delegate bool AcquireDepthImageDelegate(
        IntPtr sessionHandle, IntPtr frameHandle, ref IntPtr depthImageHandle);

    public MotionStereoDepthDataSource()
    {
        InitializeCameraIntrinsics();
    }

    public bool Initialized
    {
        get
        {
            if (!_initialized)
            {
                InitializeCameraIntrinsics();
            }

            return _initialized;
        }
    }

    public short[] DepthArray => _depthArray;
    public byte[] ConfidenceArray => _confidenceArray;
    public Matrix4x4 DepthDisplayMatrix => _depthDisplayMatrix;
    public Vector2 FocalLength => _depthCameraIntrinsics.focalLength;
    public Vector2 PrincipalPoint => _depthCameraIntrinsics.principalPoint;
    public Vector2Int ImageDimensions => _depthCameraIntrinsics.resolution;

    public void UpdateDepthTexture(ref Texture2D depthTexture)
    {
        depthTexture = _occlusionManager.environmentDepthTexture;
    }

    public void SwitchToRawDepth(bool useRawDepth)
    {
        // Enable smooth depth by default.
        _occlusionManager.environmentDepthTemporalSmoothingRequested = !_useRawDepth;
    }

    public void UpdateConfidenceTexture(ref Texture2D confidenceTexture)
    {
        confidenceTexture = _confidenceTexture;
    }

    public short[] UpdateDepthArray()
    {
        int bufferLength = _depthWidth * _depthHeight;
        if (_depthArray.Length != bufferLength)
        {
            _depthArray = new short[bufferLength];
        }

        Buffer.BlockCopy(_depthBuffer, 0, _depthArray, 0, _depthBuffer.Length);
        return _depthArray;
    }

    public byte[] UpdateConfidenceArray()
    {
        _confidenceArray = _confidenceTexture.GetRawTextureData();
        return _confidenceArray;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        _depthDisplayMatrix = eventArgs.displayMatrix.GetValueOrDefault();

        if (_cameraBackground.useCustomMaterial && _cameraBackground.customMaterial != null)
        {
            _cameraBackground.customMaterial.SetTexture(DepthSource.DepthTexturePropertyName,
                _occlusionManager.environmentDepthTexture);
            _cameraBackground.customMaterial.SetMatrix(DepthSource.DisplayTransformPropertyName,
                _depthDisplayMatrix);
        }

        UpdateEnvironmentDepthImage();
        UpdateEnvironmentDepthConfidenceImage();
    }

    private void InitializeCameraIntrinsics()
    {
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.LogWarningFormat("ARSession is not ready yet: {0}", ARSession.state);
            return;
        }

        _cameraManager ??= Object.FindObjectOfType<ARCameraManager>();
        Debug.Assert(_cameraManager);
        _cameraManager.frameReceived += OnCameraFrameReceived;

        _occlusionManager ??= Object.FindObjectOfType<AROcclusionManager>();
        Debug.Assert(_occlusionManager);

        // Enable smooth depth by default.
        _occlusionManager.environmentDepthTemporalSmoothingRequested = !_useRawDepth;

        _cameraBackground ??= Object.FindObjectOfType<ARCameraBackground>();
        Debug.Assert(_cameraBackground);

        // Gets the camera parameters to create the required number of vertices.
        if (!_cameraManager.TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics))
        {
            Debug.LogError("MotionStereoDepthDataSource: Failed to obtain camera intrinsics.");
            return;
        }

        // Scales camera intrinsics to the depth map size.
        Vector2 intrinsicsScale;
        intrinsicsScale.x = _depthWidth / (float)cameraIntrinsics.resolution.x;
        intrinsicsScale.y = _depthHeight / (float)cameraIntrinsics.resolution.y;

        var focalLength = Utilities.MultiplyVector2(cameraIntrinsics.focalLength, intrinsicsScale);
        var principalPoint =
            Utilities.MultiplyVector2(cameraIntrinsics.principalPoint, intrinsicsScale);
        var resolution = new Vector2Int(_depthWidth, _depthHeight);
        _depthCameraIntrinsics = new XRCameraIntrinsics(focalLength, principalPoint, resolution);

        if (_depthCameraIntrinsics.resolution != Vector2.zero)
        {
            _initialized = true;
            Debug.Log("MotionStereoDepthDataSource intrinsics initialized.");
        }
    }

    void UpdateEnvironmentDepthImage()
    {
        if (_occlusionManager &&
            _occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            using (image)
            {
                _depthWidth = image.width;
                _depthHeight = image.height;

                int numPixels = image.width * image.height;
                int numBytes = numPixels * image.GetPlane(0).pixelStride;

                if (_depthBuffer.Length != numBytes)
                {
                    _depthBuffer = new byte[numBytes];
                }

                image.GetPlane(0).data.CopyTo(_depthBuffer);
            }
        }
    }

    void UpdateEnvironmentDepthConfidenceImage()
    {
        if (_occlusionManager &&
            _occlusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage image))
        {
            using (image)
            {
                UpdateRawImage(ref _confidenceTexture, image, TextureFormat.R8, TextureFormat.R8);
            }
        }
    }

    static void UpdateRawImage(ref Texture2D texture, XRCpuImage cpuImage,
        TextureFormat conversionFormat, TextureFormat textureFormat)
    {
        if (texture == null || texture.width != cpuImage.width || texture.height != cpuImage.height)
        {
            texture = new Texture2D(cpuImage.width, cpuImage.height, textureFormat, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, conversionFormat);
        var rawTextureData = texture.GetRawTextureData<byte>();

        cpuImage.Convert(conversionParams, rawTextureData);
        texture.Apply();
    }
}