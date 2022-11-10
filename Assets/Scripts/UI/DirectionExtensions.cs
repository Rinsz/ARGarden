using System;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionExtensions
{
    private static readonly Dictionary<LinearRenderDirection, Vector2> Vectors = new()
    {
        [LinearRenderDirection.Left] = new Vector2(-1, 0),
        [LinearRenderDirection.Right] = new Vector2(1, 0),
        [LinearRenderDirection.Up] = new Vector2(0, 1),
        [LinearRenderDirection.Down] = new Vector2(0, -1),
    };

    public static Vector2 ToVector(this LinearRenderDirection direction) =>
        Vectors.TryGetValue(direction, out var vector)
            ? vector
            : throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported direction received.");
}