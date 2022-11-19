using System.Collections.Generic;
using System.IO;
using System.Linq;
using Models.Descriptors;
using Newtonsoft.Json;
using UnityEngine;
using static UnityConstants;

namespace Models.Loaders
{
    internal class IncludedModelsLoader : ScriptableObject
    {
        private readonly JsonSerializer serializer;
        private readonly GameObject modelSelectionMenu;

        public IncludedModelsLoader(JsonSerializer serializer, GameObject modelSelectionMenu)
        {
            this.serializer = serializer;
            this.modelSelectionMenu = modelSelectionMenu;
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
            Instantiate(modelPrefab);
            this.modelSelectionMenu.SetActive(false);
        }
    }
}