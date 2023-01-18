using System.IO;
using UnityEngine;

public static class ModelsBrowserStrings
{
    public const string Unity3dExtension = ".unity3d";
    
    public const string FavoritesKey = "Favorites";

    public const string IncludedSpritesPath = "IncludedModels/Sprites";
    public const string IncludedMetasPath = "IncludedModels/Metas";
    public const string IncludedPrefabsPath = "IncludedModels/Prefabs";

    public static readonly string CachedBundlesPath = Path.Combine(Application.persistentDataPath, "cachedBundles");
}