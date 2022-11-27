using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        // Unity умеет доставать компоненты всех детей объекта рекурсивно, но б**ть не умеет делать это нерекурсивно
        public static T[] GetComponentsInChildrenNonRecursive<T>(this Transform transform) where T : Component
        {
            return Enumerable.Range(0, transform.childCount)
                .Select(transform.GetChild)
                .Select(child => child.GetComponent<T>())
                .Where(component => component)
                .ToArray();
        }
    }
}