using System.Collections.Generic;
using System.Linq;
using Extensions;
using Lean.Touch;
using Models;
using ObjectsHandling;
using UnityEngine;
using UnityEngine.UI;
using static UiColorConstants;

[RequireComponent(typeof(LeanDragTranslate), typeof(LeanPinchScale), typeof(LeanTwistRotateAxis))]
[RequireComponent(typeof(LeanPinchZoom))]
public class ObjectsTransformController : MonoBehaviour
{
    private Dictionary<Transform, Vector3> childrenToStartPositions = new();
    private Dictionary<TransformControllerMode, Behaviour[]> componentsByMode;
    private ISet<Button> buttons;
    [SerializeField] private Button translateButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button scaleButton;

    public TransformControllerMode Mode;

    public void AddChild(Transform child)
    {
        if (childrenToStartPositions.ContainsKey(child))
        {
            Debug.Log($"Trying to add already linked child. Name: '{child.gameObject.name}'");
        }
        var oldPosition = transform.position;
        transform.position = (oldPosition * childrenToStartPositions.Count + child.transform.position) / (childrenToStartPositions.Count + 1);
        if (childrenToStartPositions.Count > 0) AdjustChildrenPositions(oldPosition);

        childrenToStartPositions.Add(child, child.position);
        child.parent = transform;
    }

    public void ReleaseChild(Transform child)
    {
        if (!childrenToStartPositions.Remove(child))
        {
            Debug.Log($"Trying to remove non linked child. Name: '{child.gameObject.name}'");
        }

        child.parent = null;

        var oldPosition = transform.position;
        if (childrenToStartPositions.Count > 0)
        {
            transform.position = (transform.position * (childrenToStartPositions.Count + 1) - child.position) / childrenToStartPositions.Count;
            AdjustChildrenPositions(oldPosition);
        }
    }

    public void RevertAllChildren()
    {
        foreach (var childToStartPosition in childrenToStartPositions)
        {
            childToStartPosition.Key.position = childToStartPosition.Value;
            childToStartPosition.Key.parent = null;
        }

        childrenToStartPositions.Clear();
    }

    public void DestroyAllChildren()
    {
        foreach (var childToStartPosition in childrenToStartPositions)
        {
            Destroy(childToStartPosition.Key.gameObject);
        }

        childrenToStartPositions.Clear();
    }

    public void ReleaseAllChildren()
    {
        foreach (var childToStartPosition in childrenToStartPositions)
        {
            childToStartPosition.Key.parent = null;
        }

        childrenToStartPositions.Clear();
    }

    public bool ContainsChild(Transform child) => childrenToStartPositions.ContainsKey(child);

    private void SetMode(TransformControllerMode mode, Button pressedButton)
    {
        pressedButton.ChangeButtonImageColor(SelectedActionButtonColor);
        buttons.Where(button => button != pressedButton).ToList().ForEach(button => button.ChangeButtonImageColor(White));

        SetEnabledLeanComponents(Mode, false);
        Mode = mode;
        SetEnabledLeanComponents(Mode, true);
    }

    private void AdjustChildrenPositions(Vector3 oldPosition)
    {
        var positionDelta = transform.position - oldPosition;
        foreach (var linkedChild in childrenToStartPositions)
            linkedChild.Key.position -= positionDelta;
    }

    private void SetEnabledLeanComponents(TransformControllerMode mode, bool enable)
    {
        if (!componentsByMode.ContainsKey(mode))
        {
            Debug.LogError($"Couldn't find components for transform controller mode '{mode}'");
            return;
        }

        foreach (var component in componentsByMode[mode])
            component.enabled = enable;
    }

    private void Start()
    {
        componentsByMode = new()
        {
            [TransformControllerMode.Translate] = new Behaviour[]
            {
                GetComponent<LeanDragTranslate>(),
                GetComponent<LeanPinchZoom>()
            },
            [TransformControllerMode.Rotate] = new Behaviour[] { GetComponent<LeanTwistRotateAxis>() },
            [TransformControllerMode.Scale] = new Behaviour[] { GetComponent<LeanPinchScale>() }
        };
        translateButton.onClick.AddListener(() => SetMode(TransformControllerMode.Translate, translateButton));
        rotateButton.onClick.AddListener(() => SetMode(TransformControllerMode.Rotate, rotateButton));
        scaleButton.onClick.AddListener(() => SetMode(TransformControllerMode.Scale, scaleButton));

        SetMode(TransformControllerMode.Translate, translateButton);
    }

    private void Awake() => buttons = new HashSet<Button> { translateButton, rotateButton, scaleButton };
}