using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class ButtonExtensions
    {
        public static void ChangeButtonImageColor(this Button button, Color32 color) =>
            button.GetComponent<Image>().color = color;
    }
}