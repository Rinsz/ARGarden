using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFader : MonoBehaviour
{
    public bool faded = false;

    private Image buttonImage;
    private TMP_Text text;
    private Color buttonColor;
    private Color textColor;
    private bool startFade = false;
    private float smoothness = 0f;
    private bool initialized = false;

    public void Start() => Initialize();

    public void Update()
    {
        if (!startFade)
            return;

        Fade(smoothness);
        if (buttonColor.a > 0.9)
            faded = true;
    }

    public void Fade(float smoothness)
    {
        if (!initialized)
            Initialize();

        this.smoothness = smoothness;
        startFade = true;

        buttonColor.a += smoothness;
        buttonImage.color = buttonColor;
        if (!text)
            return;

        textColor.a += smoothness;
        text.color = textColor;
    }

    private void Initialize()
    {
        this.startFade = false;
        this.faded = false;
        this.buttonImage = GetComponent<Image>();
        this.buttonColor = buttonImage.color;

        var text = GetComponentInChildren<TMP_Text>();
        if (text)
        {
            this.text = text;
            this.textColor = text.color;
        }

        this.initialized = true;
    }
}