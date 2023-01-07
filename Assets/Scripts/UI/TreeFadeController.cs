using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UI;
using UnityEngine;

public class TreeFadeController : MonoBehaviour
{
    public TreeFadeRenderSettings renderSettings;

    private IList<TreeFadeControllerChild> children;
    private Coroutine runningRender = null;
    private bool rendering = false;

    private void Start()
    {
        children = transform.GetComponentsInChildrenNonRecursive<TreeFadeControllerChild>()
            .Where(e => e.Fader)
            .ToArray();

        if (renderSettings.RenderOnStart)
            Fade();
        else
            SetChildrenActive(false);
    }

    public void Fade()
    {
        if (rendering)
        {
            InterruptRender();
            return;
        }

        rendering = true;
        SetChildrenActive(true);

        InitializeFaders();
        runningRender = StartCoroutine(FadeCoroutine().GetEnumerator());
    }

    private void InterruptRender()
    {
        if (!rendering)
            return;

        if (runningRender != null)
        {
            StopCoroutine(runningRender);
            runningRender = null;   
        }

        foreach (var child in children)
        {
            if (child.TreeFadeControllerComponent)
                child.TreeFadeControllerComponent.InterruptRender();

            child.gameObject.SetActive(false);
        }

        rendering = false;
    }

    private IEnumerable FadeCoroutine()
    {
        foreach (var child in children)
        {
            if (!child.Fader)
                continue;

            yield return StartCoroutine(child.Fader.Fade(renderSettings.FadeTimeSeconds).GetEnumerator());
        }

        runningRender = null;
    }

    private void InitializeFaders()
    {
        foreach (var child in children)
        {
            if (!child.Fader)
                continue;

            child.Fader.Initialize();
        }
    }

    private void SetChildrenActive(bool active)
    {
        foreach (var child in children)
        {
            child.gameObject.SetActive(active);
        }
    }
}