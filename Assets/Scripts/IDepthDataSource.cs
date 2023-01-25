using UnityEngine;

public interface IDepthDataSource
{
    bool Initialized
    {
        get;
    }

    short[] DepthArray
    {
        get;
    }

    byte[] ConfidenceArray
    {
        get;
    }

    Matrix4x4 DepthDisplayMatrix
    {
        get;
    }

    Vector2 FocalLength
    {
        get;
    }

    Vector2 PrincipalPoint
    {
        get;
    }

    Vector2Int ImageDimensions
    {
        get;
    }

    void SwitchToRawDepth(bool useRawDepth);

    void UpdateDepthTexture(ref Texture2D depthTexture);

    void UpdateConfidenceTexture(ref Texture2D confidenceTexture);

    short[] UpdateDepthArray();

    byte[] UpdateConfidenceArray();
}
