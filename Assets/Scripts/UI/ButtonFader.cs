using System.Collections;
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
    private bool initialized = false;

    public void Start() => Initialize();

    public void Fade(float smoothness) =>
        StartCoroutine(FadeCoroutine(smoothness).GetEnumerator());

    private IEnumerable FadeCoroutine(float smoothness)
    {
        if (!initialized)
            Initialize();

        while (buttonColor.a <= 0.9)
        {
            buttonColor.a += smoothness;
            buttonImage.color = buttonColor;
            if (!text)
                yield return buttonColor.a;

            textColor.a += smoothness;
            text.color = textColor;
            yield return buttonColor.a;
        }

        faded = true;
    }

    private void Initialize()
    {
        this.faded = false;
        this.buttonImage = GetComponentInChildren<Image>();
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