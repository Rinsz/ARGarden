using System;
using System.Collections.Generic;
using Lean.Touch;
using Models;
using ObjectsHandling;
using UnityEngine;

[RequireComponent(typeof(LeanDragTranslate), typeof(LeanPinchScale), typeof(LeanTwistRotateAxis))]
[RequireComponent(typeof(LeanPinchZoom))]
public class ObjectsTransformController : MonoBehaviour
{
    private HashSet<Transform> children = new();
    private Dictionary<TransformControllerMode, Behaviour[]> componentsByMode;

    public TransformControllerMode Mode;

    public void AddChild(Transform child)
    {
        if (children.Contains(child))
        {
            Debug.Log($"Trying to add already linked child. Name: '{child.gameObject.name}'");
        }
        var oldPosition = transform.position;
        transform.position = (transform.position * children.Count + child.transform.position) / (children.Count + 1);
        if (children.Count > 0) AdjustChildrenPositions(oldPosition);

        children.Add(child);
        child.parent = transform;
    }

    public void RemoveChild(Transform child)
    {
        if (!children.Remove(child))
        {
            Debug.Log($"Trying to remove non linked child. Name: '{child.gameObject.name}'");
        }

        child.parent = null;

        var oldPosition = transform.position;
        if (children.Count > 0)
        {
            transform.position = (transform.position * (children.Count + 1) - child.position) / children.Count;
            AdjustChildrenPositions(oldPosition);
        }
    }

    public bool ContainsChild(Transform child) => children.Contains(child);

    public void SetMode(TransformControllerModeHolder modeHolder)
    {
        SetEnabledLeanComponents(Mode, false);
        Mode = modeHolder.Mode;
        SetEnabledLeanComponents(Mode, true);
    }

    private void AdjustChildrenPositions(Vector3 oldPosition)
    {
        var positionDelta = transform.position - oldPosition;
        foreach (var linkedChild in children)
            linkedChild.position -= positionDelta;
    }

    private void SetEnabledLeanComponents(TransformControllerMode mode, bool enables)
    {
        if (!componentsByMode.ContainsKey(mode))
        {
            Debug.LogError($"Couldn't find components for transform controller mode '{mode}'");
            return;
        }

        foreach (var component in componentsByMode[mode])
            component.enabled = enables;
    }

    private void Start()
    {
        componentsByMode = new()
        {
            [TransformControllerMode.Position] = new Behaviour[]
            {
                GetComponent<LeanDragTranslate>(),
                GetComponent<LeanPinchZoom>()
            },
            [TransformControllerMode.Rotation] = new Behaviour[] { GetComponent<LeanTwistRotateAxis>() },
            [TransformControllerMode.Scale] = new Behaviour[] { GetComponent<LeanPinchScale>() }
        };
    }
}