using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Models.Descriptors;
using Newtonsoft.Json;
using UnityEngine;
using static UnityConstants;

namespace Models.Loaders
{
    internal class AssetBundleModelsLoader
    {
        private readonly JsonSerializer serializer;
        private readonly ObjectSpawner objectsSpawner;

        public AssetBundleModelsLoader(JsonSerializer serializer, ObjectSpawner objectsSpawner)
        {
            this.serializer = serializer;
            this.objectsSpawner = objectsSpawner;
        }

        public IEnumerable<ModelCardDescriptor> Load(ModelGroup group)
        {
            var streamingAssetsPath = CachedBundlesPath;
            var files = new DirectoryInfo(streamingAssetsPath).GetFiles();
            var loadedBundles = files
                .Where(file => file.Extension == Unity3dExtension)
                .Select(file => file.Name.Replace(Unity3dExtension, string.Empty));

            foreach (var filename in loadedBundles)
            {
                var bundle = LoadBundle(streamingAssetsPath, filename);
                var meta = LoadMeta(bundle, filename);
                if (meta == null)
                    continue;

                if (meta.ModelGroup != group && meta.ModelGroup != ModelGroup.Unknown && group != ModelGroup.Unknown)
                {
                    bundle.UnloadAsync(true);
                    continue;
                }

                var image = bundle.LoadAsset<Sprite>(filename);
                bundle.UnloadAsync(false);
                yield return new ModelCardDescriptor
                {
                    Meta = meta,
                    Image = image,
                    SelectAction = () => SelectBundleAction(meta),
                };
            }
        }

        private static AssetBundle LoadBundle(string streamingAssetsPath, string filename)
        {
            using var fs = File.OpenRead(Path.Combine(streamingAssetsPath, $"{filename}{Unity3dExtension}"));
            var bundleRequest = AssetBundle.LoadFromStreamAsync(fs);

            return bundleRequest.assetBundle;
        }

        [CanBeNull]
        private ModelMeta LoadMeta(AssetBundle bundle, string filename)
        {
            using var sr = new StringReader(bundle.LoadAsset<TextAsset>(filename).text);
            using var jsonReader = new JsonTextReader(sr);
            return serializer.Deserialize<ModelMeta>(jsonReader);
        }

        private void SelectBundleAction(ModelMeta meta)
        {
            using var fs = File.OpenRead(Path.Combine(CachedBundlesPath, $"{meta.Id}{Unity3dExtension}"));
            var bundleRequest = AssetBundle.LoadFromStreamAsync(fs);

            var bundle = bundleRequest.assetBundle;
            var obj = bundle.LoadAsset<GameObject>(meta.Id.ToString());

            objectsSpawner.SpawnObject(obj);
            bundle.UnloadAsync(false);
        }
    }
}