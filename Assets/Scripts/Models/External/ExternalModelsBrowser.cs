using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Models.Descriptors;
using Models.External;
using Models.Loaders;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Models
{
    public class ExternalModelsBrowser : MonoBehaviour
    {
        public GameObject modelDownloadCardPrefab;

        private List<ModelGroupCard> modelGroups;

        private readonly List<GameObject> createdCards = new();
        private readonly AssetBundleModelsLoader bundleModelsLoader = new(JsonSerializer.CreateDefault(), null);

        private void OnEnable() => DownloadContent();

        private void OnDisable()
        {
            foreach (var card in createdCards)
                Destroy(card);
        }

        public void DownloadContent([CanBeNull] ModelFilters filters = null)
        {
            if (createdCards.Any())
            {
                createdCards.ForEach(Destroy);
                createdCards.Clear();
            }

            var group = (ModelGroup?)filters?.groupField?.value;
            var nameFilter = filters?.nameInputField?.text;
            if (filters?.showOnlyDownloaded?.isOn == true)
            {
                LoadDownloadedCards(group ?? ModelGroup.Unknown, nameFilter);
                return;
            }

            var result = ApiClient.SendRequest(url: ApiUrlProvider.GetMetasUrl(group, nameFilter));
            result.completed += _ => CreateCards(result.webRequest.downloadHandler.text);
        }

        private void CreateCards(string metasJson)
        {
            using var tr = new StringReader(metasJson);
            using var jtr = new JsonTextReader(tr);
            var metas = JsonSerializer.CreateDefault().Deserialize<List<ModelMeta>>(jtr) ?? new();

            foreach (var meta in metas)
            {
                var rawObject = Instantiate(modelDownloadCardPrefab, this.transform, false);
                var modelDownloadCard = rawObject.GetComponent<ModelDownloadCard>();
                modelDownloadCard.meta = meta;
                modelDownloadCard.modelName.text = meta.Name;
                modelDownloadCard.SetState();
                createdCards.Add(rawObject);
                LoadImage(meta, modelDownloadCard);
            }
        }

        private void LoadDownloadedCards(ModelGroup group, string nameFilter)
        {
            var modelCardDescriptors = bundleModelsLoader.Load(group)
                .Where(descriptor => string.IsNullOrWhiteSpace(nameFilter) ||
                                     descriptor.Meta.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(descriptor => descriptor.Meta.Name);

            foreach (var modelCardDescriptor in modelCardDescriptors)
                CreateLoadedCard(modelCardDescriptor);
        }

        private void CreateLoadedCard(ModelCardDescriptor descriptor)
        {
            var (meta, image, _) = descriptor;
            var card = Instantiate(modelDownloadCardPrefab, this.transform, false);
            var modelCard = card.GetComponent<ModelDownloadCard>();
            modelCard.meta = meta;
            modelCard.modelName.text = meta.Name;
            modelCard.modelIcon.sprite = image;
            modelCard.SetState();

            createdCards.Add(card);
        }

        private static void LoadImage(ModelMeta meta, ModelDownloadCard modelDownloadCard)
        {
            var requestResult = ApiClient.SendRequestTexture(url: ApiUrlProvider.GetImageUrl(meta.Id, meta.Version));
            requestResult.completed += _ => SetImageForCard(requestResult.webRequest, modelDownloadCard);
        }

        private static void SetImageForCard(UnityWebRequest webRequest, ModelDownloadCard modelDownloadCard)
        {
            var texture = DownloadHandlerTexture.GetContent(webRequest);
            if (!modelDownloadCard.modelIcon)
                return;

            modelDownloadCard.modelIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(texture.width / 2, texture.height / 2));
        }
    }
}