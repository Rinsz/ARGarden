using UnityEngine.Networking;

namespace Models.External
{
    public class ApiClient
    {
        public static UnityWebRequestAsyncOperation SendRequestTexture(string url)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            using var certHandler = new StubCertHandler();
            request.certificateHandler = certHandler;
            return request.SendWebRequest();
        }

        public static UnityWebRequestAsyncOperation SendRequest(string url)
        {
            var request = UnityWebRequest.Get(url);
            using var certHandler = new StubCertHandler();
            request.certificateHandler = certHandler;
            return request.SendWebRequest();
        }
    }
}