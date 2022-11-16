using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityConstants;

namespace Models
{
    public class LocalModelsBrowser : MonoBehaviour
    {
        public GameObject modelCardPrefab;
        public GameObject modelSelectionMenu;
        public Button backButton;

        private List<ModelGroupCard> modelGroups;
        private readonly List<GameObject> createdCards = new();
        private HashSet<string> favorites;
        private static JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture,
        });

        public void ShowGroupContent(ModelGroupCard modelGroupCard)
        {
            DisableGroups();
            backButton.gameObject.SetActive(true);

            var modelGroupValue = modelGroupCard.modelGroup;
            var includedModels = LoadIncludedModels(modelGroupValue);
            var downloadedModels = LoadFromBundles(modelGroupValue);
            var models = includedModels.Concat(downloadedModels);

            foreach (var (meta, image, selectAction) in models)
            {
                var card = Instantiate(modelCardPrefab, this.transform, false);
                var modelCard = card.GetComponent<ModelCard>();
                modelCard.modelIcon.sprite = image;
                modelCard.modelName.text = meta.Name;
                modelCard.meta = meta;
                modelCard.selectButton.onClick.AddListener(selectAction);
                modelCard.favoriteButton.onClick.AddListener(() => modelCard.Favorite(favorites));

                if (favorites.Contains(meta.Id.ToString()))
                    modelCard.favoriteButton.image.color = new Color(122, 55, 33);

                createdCards.Add(card);
            }
        }

        private IEnumerable<ModelCardDescriptor> LoadIncludedModels(ModelGroup modelGroup)
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
                                     descriptor.Meta.ModelGroup == ModelGroup.Unknown))
                .OrderBy(descriptor => favorites.Contains(descriptor.Meta.Id.ToString()));
        }

        private IEnumerable<ModelCardDescriptor> LoadFromBundles(ModelGroup group)
        {
            var streamingAssetsPath = Application.streamingAssetsPath;
            var files = new DirectoryInfo(streamingAssetsPath).GetFiles();
            var loadedBundles = files
                .Where(file => file.Extension == Unity3dExtension)
                .Select(file => file.Name.Replace(Unity3dExtension, string.Empty))
                .OrderBy(bundleName => favorites.Contains(bundleName));

            foreach (var filename in loadedBundles)
            {
                using var fs = File.OpenRead(Path.Combine(streamingAssetsPath, $"{filename}{Unity3dExtension}"));
                var bundleRequest = AssetBundle.LoadFromStreamAsync(fs);

                var bundle = bundleRequest.assetBundle;
                using var sr = new StringReader(bundle.LoadAsset<TextAsset>(filename).text);
                using var jsonReader = new JsonTextReader(sr);
                var meta = serializer.Deserialize<ModelMeta>(jsonReader);
                if (meta == null)
                    continue;

                if (meta.ModelGroup != group && meta.ModelGroup != ModelGroup.Unknown)
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

        private void Start() => favorites = PlayerPrefs.GetString(FavoritesKey).Split(',').ToHashSet();

        private void Awake()
        {
            modelGroups = new List<ModelGroupCard>(this.GetComponentsInChildren<ModelGroupCard>());
            backButton.onClick.AddListener(() =>
            {
                ClearModelCards();
                EnableGroups();
                backButton.gameObject.SetActive(false);
            });
        }

        private void OnEnable() => EnableGroups();

        private void OnDisable()
        {
            ClearModelCards();
            DisableGroups();
            backButton.gameObject.SetActive(false);
        }

        private void ClearModelCards()
        {
            foreach (var card in createdCards)
                Destroy(card);
            Resources.UnloadUnusedAssets();
            createdCards.Clear();
        }

        private void DisableGroups()
        {
            foreach (var groupCard in modelGroups)
                groupCard.gameObject.SetActive(false);
        }

        private void EnableGroups()
        {
            if (modelGroups.Count <= 0) return;
            foreach (var group in modelGroups)
                group.gameObject.SetActive(true);
        }

        private void SelectModelAction(ModelMeta meta)
        {
            var modelPrefab = Resources.Load($"{IncludedPrefabsPath}/{meta.Id}");
            Instantiate(modelPrefab);
            this.modelSelectionMenu.SetActive(false);
        }

        private void SelectBundleAction(ModelMeta meta)
        {
            using var fs = File.OpenRead(Path.Combine(Application.streamingAssetsPath, $"{meta.Id}{Unity3dExtension}"));
            var bundleRequest = AssetBundle.LoadFromStreamAsync(fs);

            var bundle = bundleRequest.assetBundle;
            var obj = bundle.LoadAsset<GameObject>(meta.Id.ToString());

            Instantiate(obj);
            bundle.UnloadAsync(false);
            this.modelSelectionMenu.SetActive(false);
        }

        private class ModelCardDescriptor
        {
            public ModelMeta Meta { get; set; }
            public Sprite Image { get; set; }
            public UnityAction SelectAction { get; set; }

            public void Deconstruct(out ModelMeta meta, out Sprite image, out UnityAction selectAction)
            {
                meta = this.Meta;
                image = this.Image;
                selectAction = this.SelectAction;
            }
        }
    }
}