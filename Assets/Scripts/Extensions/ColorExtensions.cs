using UnityEngine;

namespace Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float newAlpha)
        {
            return new Color(color.r, color.g, color.b, newAlpha);
        }
    }
}