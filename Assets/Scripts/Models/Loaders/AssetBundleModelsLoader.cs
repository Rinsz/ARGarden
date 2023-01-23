using System.Collections.Generic;
using System.IO;
using System.Linq;
using Models.Descriptors;
using Newtonsoft.Json;
using UnityEngine;
using static ModelBrowserConstants;

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
            var assetsDirectory = new DirectoryInfo(streamingAssetsPath);
            if (!assetsDirectory.Exists)
                assetsDirectory.Create();

            var files = assetsDirectory.GetFiles();
            var loadedBundles = files
                .GroupBy(file => file.Name.Split('.')[0])
                .Where(gFiles => gFiles.Count() == 3)
                .Select(filesGroup => (
                    Meta: filesGroup.First(f => f.Extension == ".json"),
                    Image: filesGroup.First(f => f.Extension == ".jpg")));

            foreach (var bundleInfo in loadedBundles)
            {
                var (metaFileInfo, imageFileInfo) = bundleInfo;

                var meta = LoadMeta(metaFileInfo);
                if (meta == null || (meta.ModelGroup != group &&
                                    meta.ModelGroup != ModelGroup.Unknown &&
                                    group != ModelGroup.Unknown))
                {
                    continue;
                }

                yield return new ModelCardDescriptor
                {
                    Meta = meta,
                    Image = LoadImage(imageFileInfo),
                    SelectAction = () => SelectBundleAction(meta),
                };
            }
        }

        private static Sprite LoadImage(FileInfo image)
        {
            var img = File.ReadAllBytes(image.FullName);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(img);
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(tex.width / 2, tex.height / 2));
        }

        private ModelMeta LoadMeta(FileInfo meta)
        {
            var rawMeta = meta.OpenText().ReadToEnd();
            using var sr = new StringReader(rawMeta);
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