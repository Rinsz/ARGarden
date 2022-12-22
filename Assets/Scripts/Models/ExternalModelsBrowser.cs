using System.Collections.Generic;
using System.IO;
using Models.Descriptors;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Models
{
    public class ExternalModelsBrowser : MonoBehaviour
    {
        public GameObject ModelDownloadCardPrefab;

        private List<ModelGroupCard> modelGroups;

        private readonly List<GameObject> createdCards = new();

        private void OnEnable() => DownloadContent();

        private void OnDisable()
        {
            foreach (var card in createdCards)
                Destroy(card);
        }

        public void DownloadContent()
        {
            var metasUrl = ApiUrlProvider.GetMetasUrl();
            var metasRequest = UnityWebRequest.Get(metasUrl);
            using var certHandler = new StubCertHandler();
            metasRequest.certificateHandler = certHandler;
            var requestResult = metasRequest.SendWebRequest();
            requestResult.completed += _ => CreateCards(requestResult.webRequest.downloadHandler.text);
        }

        private void CreateCards(string metasJson)
        {
            using var tr = new StringReader(metasJson);
            using var jtr = new JsonTextReader(tr);
            var metas = JsonSerializer.CreateDefault().Deserialize<List<ModelMeta>>(jtr);

            foreach (var meta in metas)
            {
                var rawObject = Instantiate(ModelDownloadCardPrefab, this.transform, false);
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