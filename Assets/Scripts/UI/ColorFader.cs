using System.Collections;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ColorFader : MonoBehaviour
{
    private MaskableGraphic[] allGraphics;

    public void Awake()
    {
        allGraphics = GetComponents<MaskableGraphic>()
            .Concat(transform.GetComponentsInChildren<MaskableGraphic>())
            .ToArray();
    }
 
    public IEnumerable Fade(float fadeTimeSeconds)
    {
        var startTime = Time.time;
        var fadeState = 0f;
        do
        {
            fadeState = (Time.time - startTime) / fadeTimeSeconds;
            foreach (var graphic in allGraphics)
            {
                graphic.color = graphic.color.WithAlpha(fadeState);
            }

            yield return null;
        } while (fadeState < 1);
    }

    public void Initialize()
    {
        foreach (var maskableGraphic in allGraphics)
        {
            maskableGraphic.color = maskableGraphic.color.WithAlpha(0);
        }
    }
}