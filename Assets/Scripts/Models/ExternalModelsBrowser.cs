using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Models.Descriptors;
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

        private void OnEnable() => DownloadContent();

        private void OnDisable()
        {
            foreach (var card in createdCards)
                Destroy(card);
        }

        public void DownloadContent([CanBeNull] ModelFilters filters = null)
        {
            var group = (ModelGroup?)filters?.groupField?.value;
            var nameFilter = filters?.nameInputField?.text;

            var metasUrl = ApiUrlProvider.GetMetasUrl(group, nameFilter);
            var metasRequest = UnityWebRequest.Get(metasUrl);
            using var certHandler = new StubCertHandler();
            metasRequest.certificateHandler = certHandler;
            var requestResult = metasRequest.SendWebRequest();
            if (createdCards.Any())
            {
                createdCards.ForEach(Destroy);
                createdCards.Clear();
            }

            requestResult.completed += _ => CreateCards(requestResult.webRequest.downloadHandler.text);
        }

        private void CreateCards(string metasJson)
        {
            using var tr = new StringReader(metasJson);
            using var jtr = new JsonTextReader(tr);
            var metas = JsonSerializer.CreateDefault().Deserialize<List<ModelMeta>>(jtr);

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

        private static void LoadImage(ModelMeta meta, ModelDownloadCard modelDownloadCard)
        {
            var imageUrl = ApiUrlProvider.GetImageUrl(meta.Id, meta.Version);
            var imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            using var ch = new StubCertHandler();
            imageRequest.certificateHandler = ch;
            var imageRequestResult = imageRequest.SendWebRequest();
            imageRequestResult.completed += _ => SetImageForCard(imageRequestResult.webRequest, modelDownloadCard);
        }

        private static void SetImageForCard(UnityWebRequest webRequest, ModelDownloadCard modelDownloadCard)
        {
            var texture = DownloadHandlerTexture.GetContent(webRequest);
            var image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(texture.width / 2, texture.height / 2));

            modelDownloadCard.modelIcon.sprite = image;
        }

        // TODO Issue cert for api and remove stub
        private class StubCertHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }
    }
}