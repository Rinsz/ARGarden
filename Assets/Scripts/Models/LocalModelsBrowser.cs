using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Models.Descriptors;
using Models.Loaders;
using Newtonsoft.Json;
using UnityEngine;
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

        private IncludedModelsLoader includedModelsLoader;
        private AssetBundleModelsLoader assetBundleModelsLoader;
        private HashSet<string> favorites;

        public void ShowGroupContent(ModelGroupCard modelGroupCard)
        {
            DisableGroups();
            backButton.gameObject.SetActive(true);

            var modelGroupValue = modelGroupCard.modelGroup;
            var includedModels = includedModelsLoader.Load(modelGroupValue);
            var downloadedModels = assetBundleModelsLoader.Load(modelGroupValue);
            var models = includedModels
                .Concat(downloadedModels)
                .OrderBy(descriptor => favorites.Contains(descriptor.Meta.Id.ToString()));

            foreach (var descriptor in models)
                CreateModelCard(descriptor);
        }

        private void Start()
        {
            favorites = PlayerPrefs.GetString(FavoritesKey).Split(',').ToHashSet();
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture });
            includedModelsLoader = new(serializer, modelSelectionMenu);
            assetBundleModelsLoader = new(serializer, modelSelectionMenu);
        }

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

        private void CreateModelCard(ModelCardDescriptor descriptor)
        {
            var (meta, image, selectAction) = descriptor;
            var card = Instantiate(modelCardPrefab, this.transform, false);
            var modelCard = card.GetComponent<ModelCard>();
            modelCard.modelIcon.sprite = image;
            modelCard.modelName.text = meta.Name;
            modelCard.meta = meta;
            modelCard.selectButton.onClick.AddListener(selectAction);
            modelCard.favoriteButton.onClick.AddListener(() => modelCard.Favorite(ref favorites));

            if (favorites.Contains(meta.Id.ToString()))
                modelCard.favoriteButton.image.color = new Color(122, 55, 33);

            createdCards.Add(card);
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
    }
}