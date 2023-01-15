using System.IO;
using Newtonsoft.Json;
using TMPro;
using UI;
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
        public LoadBar downloadBar;
        public Button downloadButton;
        public Button deleteButton;
        internal ModelMeta meta;

        private static string CachePath => Path.Combine(Application.persistentDataPath, "cachedBundles");

        internal void SetState()
        {
            var cachePath = CachePath;
            var bundlePath = Path.Combine(cachePath, $"{meta.Id}{Unity3dExtension}");
            var metaPath = Path.Combine(cachePath, $"{meta.Id}.json");

            var filesExist = File.Exists(bundlePath) && File.Exists(metaPath);
            downloadButton.gameObject.SetActive(!filesExist);
            deleteButton.gameObject.SetActive(filesExist);
        }

        public void Download()
        {
            while (!Caching.ready)
            {
                Debug.Log("Cache is not ready.");
            }

            var bundleUrl = ApiUrlProvider.GetBundleUrl(meta.Id, meta.Version);
            var bundleRequest = UnityWebRequest.Get(bundleUrl);
            using var certHandler = new StubCertHandler();
            bundleRequest.certificateHandler = certHandler;
            var requestResult = bundleRequest.SendWebRequest();
            downloadBar.RunLoader(requestResult);

            requestResult.completed += _ =>
            {
                var cachePath = CachePath;
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
                downloadButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
            };
        }

        public void Delete()
        {
            var cachePath = CachePath;
            var bundlePath = Path.Combine(cachePath, $"{meta.Id}{Unity3dExtension}");
            if (File.Exists(bundlePath))
                File.Delete(bundlePath);

            var metaPath = Path.Combine(cachePath, $"{meta.Id}.json");
            if (File.Exists(metaPath))
                File.Delete(metaPath);
            
            downloadButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }

        // TODO Issue cert for api and remove stub
        private class StubCertHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
    }
}