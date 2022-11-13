using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildAssetBundles
    {
        [MenuItem("Assets/Build Asset Bundles")]
        public static void Build()
        {
            var manifest = BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
            foreach (var bundle in manifest.GetAllAssetBundles())
                Debug.Log($"Built bundle: {bundle}");
        }
    }
}