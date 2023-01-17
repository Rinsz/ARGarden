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
        public Button closeButton;
        public UnityEvent onClosedWithoutSpawn = new();

        [SerializeField] private GameObject menu;
        [SerializeField] private Transform content;
        [SerializeField] private ModelGroupCard[] groupCards;
        [SerializeField] private ObjectSpawner objectSpawner;

        private readonly List<GameObject> createdCards = new();

        private IncludedModelsLoader includedModelsLoader;
        private AssetBundleModelsLoader assetBundleModelsLoader;
        private HashSet<string> favorites;

        public void SetMenuActive(bool active) => menu.SetActive(active);
        
        private void Start()
        {
            favorites = PlayerPrefs.GetString(FavoritesKey).Split(',').ToHashSet();

            objectSpawner.OnSpawned.AddListener(_ => SetMenuActive(false));
        }

        private void Awake()
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture });
            includedModelsLoader = new(serializer, objectSpawner);
            assetBundleModelsLoader = new(serializer, objectSpawner);
            closeButton.onClick.AddListener(() =>
            {
                ClearModelCards();
                SetMenuActive(false);
                onClosedWithoutSpawn.Invoke();
            });
            backButton.onClick.AddListener(() =>
            {
                SetGroupsActive(true);
                ClearModelCards();
            });
            foreach (var groupCard in groupCards)
            {
                groupCard.OnGroupChoose.AddListener(ShowGroupContent);
            }
        }

        private void ShowGroupContent(ModelGroup modelGroup)
        {
            SetGroupsActive(false);
            backButton.gameObject.SetActive(true);

            var includedModels = includedModelsLoader.Load(modelGroup).ToArray();
            var downloadedModels = assetBundleModelsLoader.Load(modelGroup).ToArray();
            Debug.Log(includedModels.Length);
            Debug.Log(downloadedModels.Length);
            var cardDescriptors = includedModels
                .Concat(downloadedModels)
                .OrderBy(descriptor => favorites.Contains(descriptor.Meta.Id.ToString()))
                .ThenBy(descriptor => descriptor.Meta.Name);

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
            Debug.Log("b " + meta.Name);
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
            if (groupCards.Length <= 0) return;
            foreach (var groupCard in groupCards)
                groupCard.gameObject.SetActive(isActive);
        }
    }
}
