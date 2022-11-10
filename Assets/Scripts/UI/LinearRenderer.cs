using System;
using UnityEngine;

[Serializable]
public class LinearRenderer
{
    public LinearRenderDirection direction = LinearRenderDirection.Down;
    public float defaultButtonsSpacing = 5f;
    public int buttonOffset = 0;

    [HideInInspector]
    public float buttonsSpacing = 5f;

    public void AdjustSpacingToScreenSize(Vector2 referenceScreenSize)
    {
        var referencedScreen = referenceScreenSize.x + referenceScreenSize.y;
        var actualScreen = Screen.width + Screen.height;
        buttonsSpacing = (defaultButtonsSpacing * actualScreen) / referencedScreen;
    }
}