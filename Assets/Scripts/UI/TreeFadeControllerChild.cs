using JetBrains.Annotations;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(ColorFader))]
    public class TreeFadeControllerChild : MonoBehaviour
    {
        public ColorFader Fader { get; private set; }

        [CanBeNull]
        public TreeFadeController TreeFadeControllerComponent { get; private set; }

        private void Awake()
        {
            Fader = GetComponent<ColorFader>();
            TreeFadeControllerComponent = GetComponent<TreeFadeController>();
        }
    }
}