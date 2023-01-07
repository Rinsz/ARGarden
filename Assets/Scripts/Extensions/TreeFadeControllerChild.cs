using JetBrains.Annotations;
using UnityEngine;

namespace Extensions
{
    [RequireComponent(typeof(ColorFader))]
    public class TreeFadeControllerChild : MonoBehaviour
    {
        public ColorFader Fader { get; private set; }

        [CanBeNull]
        public TreeFadeController TreeFadeControllerComponent { get; private set; }

        private void Start()
        {
            Fader = GetComponent<ColorFader>();
            TreeFadeControllerComponent = GetComponent<TreeFadeController>();
        }
    }
}