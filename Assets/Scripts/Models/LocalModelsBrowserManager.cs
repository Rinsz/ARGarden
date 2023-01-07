using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Models.Descriptors;
using Models.Loaders;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityConstants;

namespace Models
{
    public class LocalModelsBrowserManager : MonoBehaviour
    {
        public GameObject modelCardPrefab;
        public Button backButton;
        public UnityEvent onClosedWithoutSpawn = new();

        [SerializeField] private GameObject menu;
        [SerializeField] private Transform content;
        [SerializeField] private ObjectSpawnController objectSpawnController;

        private List<ModelGroupCard> modelGroups;

        private readonly List<GameObject> createdCards = new();

        private IncludedModelsLoader includedModelsLoader;
        private AssetBundleModelsLoader assetBundleModelsLoader;
        private HashSet<string> favorites;

        public void SetMenuActive(bool active) => menu.SetActive(active);
        
        private void Start()
        {
            favorites = PlayerPrefs.GetString(FavoritesKey).Split(',').ToHashSet();

            objectSpawnController.OnSpawned.AddListener(_ => SetMenuActive(false));
        }

        private void Awake()
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture });
            includedModelsLoader = new(serializer, objectSpawnController);
            assetBundleModelsLoader = new(serializer, objectSpawnController);
            modelGroups = new List<ModelGroupCard>(content.GetComponentsInChildren<ModelGroupCard>());
            backButton.onClick.AddListener(() =>
            {
                ClearModelCards();
                SetMenuActive(false);
                onClosedWithoutSpawn.Invoke();
            });
        }

        public void ShowGroupContent(ModelGroupCard modelGroupCard)
        {
            SetGroupsActive(false);
            backButton.gameObject.SetActive(true);

            var modelGroupValue = modelGroupCard.modelGroup;
            var includedModels = includedModelsLoader.Load(modelGroupValue);
            var downloadedModels = assetBundleModelsLoader.Load(modelGroupValue);
            var cardDescriptors = includedModels
                .Concat(downloadedModels)
                .OrderBy(descriptor => favorites.Contains(descriptor.Meta.Id.ToString()));

            foreach (var descriptor in cardDescriptors)
                CreateModelCard(descriptor);
        }

        private void CreateModelCard(ModelCardDescriptor descriptor)
        {
            var (meta, image, selectAction) = descriptor;
            var card = Instantiate(modelCardPrefab, content, false);
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

        private void SetGroupsActive(bool isActive)
        {
            if (modelGroups.Count <= 0) return;
            foreach (var groupCard in modelGroups)
                groupCard.gameObject.SetActive(isActive);
        }
    }
}