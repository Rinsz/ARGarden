using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LeanFingerTap), typeof(Outline))]
public class TranslatedObject : MonoBehaviour
{
    private Outline outLine;
    public UnityEvent OnTap { get; } = new();

    private void Awake()
    {
        GetComponent<LeanFingerTap>().OnFinger.AddListener(finger =>
        {
            if (Physics.Raycast(finger.GetRay(), out var hit) && hit.transform == transform)
                OnTap.Invoke();
        });
        outLine = GetComponent<Outline>();
    }

    public void SetOutLineEnabled(bool flag) => outLine.enabled = flag;
}