using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LeanFingerTap))]
public class TranslatedObject : MonoBehaviour
{
    public UnityEvent OnTap { get; } = new();

    private void Start()
    {
        GetComponent<LeanFingerTap>().OnFinger.AddListener(finger =>
        {
            if (Physics.Raycast(finger.GetRay(), out var hit) && hit.transform == transform)
                OnTap.Invoke();
        });
    }
}