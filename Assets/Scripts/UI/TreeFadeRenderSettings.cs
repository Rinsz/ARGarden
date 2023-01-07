using System;
using UnityEngine;

[Serializable]
public class TreeFadeRenderSettings
{
    [Range(0.01f, 10)]
    public float FadeTimeSeconds = 0.2f;
    public bool RenderOnStart = false;
}