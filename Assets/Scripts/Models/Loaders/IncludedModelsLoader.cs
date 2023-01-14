using System.Collections.Generic;
using System.IO;
using System.Linq;
using Models.Descriptors;
using Newtonsoft.Json;
using UnityEngine;
using static UnityConstants;

namespace Models.Loaders
{
    internal class IncludedModelsLoader
    {
        private readonly JsonSerializer serializer;
        private readonly ObjectSpawner objectsSpawner;

        public IncludedModelsLoader(JsonSerializer serializer, ObjectSpawner objectsSpawner)
        {
            this.serializer = serializer;
            this.objectsSpawner = objectsSpawner;
        }

        public IEnumerable<ModelCardDescriptor> Load(ModelGroup modelGroup)
        {
            var availableModels = Resources.LoadAll<Sprite>(IncludedSpritesPath).OrderBy(model => model.name);
            return Resources.LoadAll<TextAsset>(IncludedMetasPath)
                .OrderBy(asset => asset.name)
                .Select(asset =>
                {
                    using var sr = new StringReader(asset.text);
                    using var jsonReader = new JsonTextReader(sr);
                    return serializer.Deserialize<ModelMeta>(jsonReader);
                })
                .Zip(availableModels, (meta, image) => new ModelCardDescriptor
                {
                    Meta = meta,
                    Image = image,
                    SelectAction = () => SelectModelAction(meta),
                })
                .Where(descriptor => descriptor.Meta != null &&
                                     (descriptor.Meta.ModelGroup == modelGroup ||
                                      descriptor.Meta.ModelGroup == ModelGroup.Unknown));
        }
        
        private void SelectModelAction(ModelMeta meta)
        {
            var modelPrefab = Resources.Load($"{IncludedPrefabsPath}/{meta.Id}");
            objectsSpawner.SpawnObject((GameObject) modelPrefab);
        }
    }
}