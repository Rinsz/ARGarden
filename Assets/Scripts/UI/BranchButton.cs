using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonScaler), typeof(RenderSettings), typeof(LinearRenderer))]
public class BranchButton : MonoBehaviour
{
    public GameObject[] buttonPrefabs;

    [HideInInspector] 
    public List<GameObject> buttons;

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
        buttons = new List<GameObject>();
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

        RenderLinearFade();
    }

    public void RenderButtons()
    {
        renderSettings.rendering = true;
        foreach (var button in buttons)
            Destroy(button);
        buttons.Clear();

        ClearChildBranchedButtons();

        foreach (var prefab in buttonPrefabs)
        {
            var button = Instantiate(prefab, transform, true);
            button.transform.position = transform.position;

            var color = button.GetComponent<Image>().color;
            color.a = 0;
            button.GetComponent<Image>().color = color;

            var text = button.GetComponentInChildren<TMP_Text>();
            if (text)
            {
                color = text.color;
                color.a = 0;
                button.GetComponentInChildren<TMP_Text>().color = color;
            }

            buttons.Add(button);
        }

        SetButtonsPosition();

        renderSettings.created = true;
    }

    private void ClearChildBranchedButtons()
    {
        var childBranches = FindObjectsOfType<BranchButton>();
        foreach (var childBranch in childBranches)
        {
            if (childBranch.transform.parent != transform.parent)
                continue;

            foreach (var button in childBranch.buttons)
                Destroy(button);
            childBranch.buttons.Clear();
        }
    }

    private void RenderLinearFade()
    {
        for (var i = 0; i < buttons.Count; i++)
        {
            var previousFader = i - 1 > 0
                ? buttons[i - 1].GetComponent<ButtonFader>()
                : null;
            var fader = buttons[i].GetComponent<ButtonFader>();

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
            var rect = buttons[i].GetComponent<RectTransform>();
            rect.sizeDelta = sizeDelta;

            target.x = direction.x * ((i + buttonOffset) * (sizeDelta.x + buttonsSpacing)) + position.x;
            target.y = direction.y * ((i + buttonOffset) * (sizeDelta.y + buttonsSpacing)) + position.y;
            target.z = 0;
            
            buttons[i].transform.position = target;
        }
    }
}