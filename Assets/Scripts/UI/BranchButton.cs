using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonScaler), typeof(RenderSettings), typeof(LinearRenderer))]
public class BranchButton : MonoBehaviour
{
    public GameObject[] buttonPrefabs;

    [HideInInspector] 
    public List<(GameObject Button, ButtonFader Fader)> buttons;

    public ScalingMode scalingMode;
    public Vector2 referenceButtonSize;
    public Vector2 referenceScreenSize;

    public ButtonScaler buttonScaler;
    public RenderSettings renderSettings;
    public LinearRenderer linearRenderer;

    private int screenWidth = 0;
    private int screenHeight = 0;

    private void Start()
    {
        buttons = new List<(GameObject Button, ButtonFader Fader)>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        buttonScaler.Initialize(referenceButtonSize, referenceScreenSize, scalingMode);
        linearRenderer.AdjustSpacingToScreenSize(buttonScaler.referenceScreenSize);

        if (renderSettings.renderOnStart)
            RenderButtons();
    }

    private void Update()
    {
        if (Screen.width != screenWidth || Screen.height != screenHeight)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            buttonScaler.Initialize(referenceButtonSize, referenceScreenSize, scalingMode);
            linearRenderer.AdjustSpacingToScreenSize(buttonScaler.referenceScreenSize);
            RenderButtons();
        }

        if (!renderSettings.rendering)
            return;

        if (!renderSettings.created)
            RenderButtons();

        if (buttons.Any(button => !button.Fader.faded))
            RenderLinearFade();
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
        var childBranches = FindObjectsOfType<BranchButton>();
        foreach (var childBranch in childBranches)
        {
            if (childBranch.transform.parent != transform.parent)
                continue;

            foreach (var (button, _) in childBranch.buttons)
                Destroy(button);
            childBranch.buttons.Clear();
        }
    }

    private void RenderLinearFade()
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            var previousFader = i - 1 > 0
                ? buttons[i - 1].Fader
                : null;
            var fader = buttons[i].Fader;

            if (previousFader)
            {
                if (!previousFader.faded)
                    continue;
                if (fader)
                    fader.Fade(renderSettings.fadeSmoothness);
            }
            else
            {
                if (fader)
                    fader.Fade(renderSettings.fadeSmoothness);
                else
                    throw new Exception($"Fading button should have {nameof(ButtonFader)} component");
            }
        }
    }

    private void SetButtonsPosition()
    {
        var sizeDelta = buttonScaler.branchedButtonSize;
        var direction = linearRenderer.direction.ToVector();
        var buttonOffset = linearRenderer.buttonOffset;
        var buttonsSpacing = linearRenderer.buttonsSpacing;
        var position = transform.position;

        for (var i = 0; i < buttons.Count; i++)
        {
            Vector3 target;
            buttons[i].Button.GetComponent<RectTransform>().sizeDelta = sizeDelta;

            target.x = direction.x * ((i + buttonOffset) * (sizeDelta.x + buttonsSpacing)) + position.x;
            target.y = direction.y * ((i + buttonOffset) * (sizeDelta.y + buttonsSpacing)) + position.y;
            target.z = 0;
            
            buttons[i].Button.transform.position = target;
        }
    }
}