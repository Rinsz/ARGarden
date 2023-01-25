using UnityEngine;

public class DepthTarget : MonoBehaviour
{
    /// <summary>
    /// Type of depth texture to attach to the material.
    /// </summary>
    public bool UseRawDepth = false;

    /// <summary>
    /// Flag to set the depth texture as mainTexture.
    /// </summary>
    public bool SetAsMainTexture = false;

    /// <summary>
    /// Material that get a depth texture assigned.
    /// </summary>
    public Material DepthTargetMaterial;

    private void OnEnable()
    {
        // Takes the material of the object's renderer, if no DepthTargetMaterial is explicitly set.
        if (DepthTargetMaterial == null)
        {
            var renderer = GetComponent<Renderer>();

            if (renderer != null)
            {
                DepthTargetMaterial = renderer.sharedMaterial;
            }
        }

        DepthSource.SwitchToRawDepth(UseRawDepth);
        DepthSource.AddDepthTarget(this);
    }

    private void OnDisable()
    {
        DepthSource.RemoveDepthTarget(this);
    }
}