using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityConstants;

namespace Models.Descriptors
{
    public class ModelDownloadCard : MonoBehaviour
    {
        public Image modelIcon;
        public TMP_Text modelName;
        internal ModelMeta meta;

        public void Download()
        {
            while (!Caching.ready)
            {
                Debug.Log("Cache is not ready.");
            }

            var bundleRequest = UnityWebRequest.Get($"https://localhost:7051/api/models/bundles/{meta.Id}/{meta.Version}");
            using var certHandler = new StubCertHandler();
            bundleRequest.certificateHandler = certHandler;
            var requestResult = bundleRequest.SendWebRequest();
            requestResult.completed += _ =>
            {
                var cachePath = Path.Combine(Application.persistentDataPath, "cachedBundles");
                if (!Directory.Exists(cachePath))
                    Directory.CreateDirectory(cachePath);
                var bytes = requestResult.webRequest.downloadHandler.data;
                using var fileStream = new FileStream(Path.Combine(cachePath, $"{meta.Id}{Unity3dExtension}"), FileMode.Create);
                fileStream.Write(bytes, 0, bytes.Length);

                
                using var metaFileStream = new FileStream(Path.Combine(cachePath, $"{meta.Id}.json"), FileMode.Create);
                using var sw = new StreamWriter(metaFileStream);
                JsonSerializer.Create().Serialize(sw, meta);

                Debug.Log($"Download completed: {meta.Name}/{meta.ModelGroup}");
                Debug.Log($"Saved to: {Caching.currentCacheForWriting.path}");
            };
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