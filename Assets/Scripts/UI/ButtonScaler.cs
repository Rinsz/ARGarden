using System;
using UI;
using UnityEngine;

[Serializable]
public class ButtonScaler
{
    private ScalingMode scaleMode;
    private Vector2 referenceSize;

    [HideInInspector]
    public Vector2 referenceScreenSize;
    public Vector2 branchedButtonSize;

    public void Initialize(Vector2 referenceSize, Vector2 referenceScreenSize, ScalingMode scalingMode)
    {
        this.scaleMode = scalingMode;
        this.referenceSize = referenceSize;
        this.referenceScreenSize = referenceScreenSize;
        SetBranchedButtonSize(scalingMode);
    }

    private void SetBranchedButtonSize(ScalingMode newScaleMode)
    {
        switch (newScaleMode)
        {
            case ScalingMode.MatchWidthHeight:
                branchedButtonSize.x = (this.referenceSize.x * Screen.width) / referenceScreenSize.x;
                branchedButtonSize.y = branchedButtonSize.x;
                break;
            case ScalingMode.IndependentWidthHeight:
                branchedButtonSize.x = (this.referenceSize.x * Screen.width) / referenceScreenSize.x;
                branchedButtonSize.y = (this.referenceSize.y * Screen.height) / referenceScreenSize.y;
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(scaleMode),
                    $"Unsupported {nameof(ScalingMode)} value received. Value: {newScaleMode}");
        }
    }
}