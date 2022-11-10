using System;
using UnityEngine;

[Serializable]
public class RenderSettings
{
    public float fadeSmoothness = 0.01f;
    public bool renderOnStart = false;

    [HideInInspector]
    public bool rendering = false;

    [HideInInspector]
    public bool created = false;
}