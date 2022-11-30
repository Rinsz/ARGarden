using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static T[] GetComponentsInChildrenNonRecursive<T>(this Transform transform) where T : Component
        {
            return Enumerable.Range(0, transform.childCount)
                .Select(transform.GetChild)
                .Select(child => child.GetComponent<T>())
                .ToArray();
        }
    }
}