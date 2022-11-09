using System.Collections;
using System.Collections.Concurrent;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonScaler), typeof(RenderSettings), typeof(LinearRenderer))]
public class BranchButton : MonoBehaviour
{
    public GameObject[] buttonPrefabs;

    public ScalingMode scalingMode;
    public Vector2 referenceButtonSize;
    public Vector2 referenceScreenSize;

    public ButtonScaler buttonScaler;
    public RenderSettings renderSettings;
    public LinearRenderer linearRenderer;

    private ConcurrentBag<(GameObject Button, ButtonFader Fader)> buttons;
    private int screenWidth = 0;
    private int screenHeight = 0;
    private Coroutine runningRender = null;

    private void Start()
    {
        buttons = new ConcurrentBag<(GameObject Button, ButtonFader Fader)>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        buttonScaler.Initialize(referenceButtonSize, referenceScreenSize, scalingMode);
        linearRenderer.AdjustSpacingToScreenSize(buttonScaler.referenceScreenSize);

        if (renderSettings.renderOnStart)
            RenderButtons();
    }

    public void RenderButtons()
    {
        if (renderSettings.created || renderSettings.rendering)
        {
            renderSettings.created = false;
            renderSettings.rendering = false;
            ClearChildBranchedButtons();
            return;
        }

        renderSettings.rendering = true;
        foreach (var (button, _) in buttons)
            Destroy(button);
        buttons.Clear();

        ClearChildBranchedButtons();

        foreach (var prefab in buttonPrefabs)
        {
            var button = Instantiate(prefab, transform, true);
            button.transform.position = transform.position;

            var color = button.GetComponentInChildren<Image>().color;
            color.a = 0;
            button.GetComponentInChildren<Image>().color = color;

            var text = button.GetComponentInChildren<TMP_Text>();
            if (text)
            {
                color = text.color;
                color.a = 0;
                button.GetComponentInChildren<TMP_Text>().color = color;
            }

            buttons.Add((button, button.GetComponent<ButtonFader>()));
        }

        SetButtonsPosition();
        runningRender = StartCoroutine(RenderLinearFade().GetEnumerator());
        renderSettings.created = true;
    }

    public void ClearAllChild()
    {
        var childBranches = FindObjectsOfType<BranchButton>();
        foreach (var childBranch in childBranches)
        {
            foreach (var (button, _) in childBranch.buttons)
                Destroy(button);
            childBranch.buttons.Clear();
            childBranch.renderSettings.rendering = false;
            childBranch.renderSettings.created = false;
        }
    }

    private void ClearChildBranchedButtons()
    {
        if (runningRender != null)
        {
            StopCoroutine(runningRender);
            runningRender = null;   
        }

        var childBranches = FindObjectsOfType<BranchButton>();
        foreach (var childBranch in childBranches)
        {
            if (childBranch.transform.parent != transform.parent)
                continue;

            foreach (var (button, _) in childBranch.buttons)
                Destroy(button);
            childBranch.buttons.Clear();
            childBranch.renderSettings.rendering = false;
            childBranch.renderSettings.created = false;
        }
    }

    private IEnumerable RenderLinearFade()
    {
        foreach (var (_, fader) in buttons)
        {
            if (!fader)
                continue;

            fader.Fade(renderSettings.fadeSmoothness);
            while (!fader.faded)
                yield return fader.faded;
        }

        runningRender = null;
    }

    private void SetButtonsPosition()
    {
        var sizeDelta = buttonScaler.branchedButtonSize;
        var direction = linearRenderer.direction.ToVector();
        var buttonOffset = linearRenderer.buttonOffset;
        var buttonsSpacing = linearRenderer.buttonsSpacing;
        var position = transform.position;

        var buttonsArray = this.buttons.ToArray();
        for (var i = 0; i < buttons.Count; i++)
        {
            Vector3 target;
            buttonsArray[i].Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;

            target.x = direction.x * ((i + buttonOffset) * (sizeDelta.x + buttonsSpacing)) + position.x;
            target.y = direction.y * ((i + buttonOffset) * (sizeDelta.y + buttonsSpacing)) + position.y;
            target.z = 0;
            
            buttonsArray[i].Button.transform.position = target;
        }
    }
}