using UnityEngine;

namespace Extensions
{
    public class ToastSender : MonoBehaviour
    {
        private AndroidJavaObject currentActivity;
        private static ToastSender instance;

        public static void Send(string message)
        {
            if (!instance)
            {
                Debug.LogError($"{nameof(ToastSender)} instance should be created to send toasts");
                return;
            }

            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.LogError($"Can't send toast on platform '{Application.platform}'");
                return;
            }

            instance.SendByInstance(message);
        }

        private void SendByInstance(string message)
        {
            var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            var toastClass = new AndroidJavaClass("android.widget.Toast");
            var javaString = new AndroidJavaObject("java.lang.String", message);
            var toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString,
                toastClass.GetStatic<int>("LENGTH_SHORT"));
            toast.Call("show");
        }

        private void Awake()
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            instance = this;
        }
    }
}