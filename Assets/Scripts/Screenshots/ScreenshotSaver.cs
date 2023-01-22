using System;
using System.Collections;
using System.Linq;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Screenshots
{
    public class ScreenshotSaver : MonoBehaviour
    {
        public Button button;
 
        private void Awake()
        {
            button.onClick
                .AddListener(() => StartCoroutine(CaptureScreen()));
        }

        private IEnumerator CaptureScreen()
        {
            yield return null;

            var componentsToHide = GetComponentsToHide();
            foreach (var obj in componentsToHide)
                obj.enabled = false;

            yield return new WaitForEndOfFrame();

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var camera = Camera.main!;

            if (Application.platform == RuntimePlatform.Android)
            {
                var screenshot = TakeScreenshot(screenWidth, screenHeight, camera);
                SaveScreenshotToGallery(screenshot);
            }
            else
                ScreenCapture.CaptureScreenshot(GetFileName());

            foreach (var obj in componentsToHide)
                obj.enabled = true;
        }

        private static Texture2D TakeScreenshot(int screenWidth, int screenHeight, Camera camera)
        {
            var rt = new RenderTexture(screenWidth, screenHeight, 24);
            camera.targetTexture = rt;
            var screenshot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            return screenshot;
        }

        private void SaveScreenshotToGallery(Texture2D screenshot)
        {
            var permission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);

            if (permission == NativeGallery.Permission.Granted)
            {
                var fileName = GetFileName();
                NativeGallery.SaveImageToGallery(screenshot, "ARGarden", fileName);
                ToastSender.Send("Снимок экрана сохранён в галерею");
            }
            else
                ToastSender.Send("Не удалось сохранить снимок экрана");
        }

        private Behaviour[] GetComponentsToHide()
        {
            var canvas = FindObjectOfType<Canvas>();
            var trackables = FindObjectsOfType<ARPlaneMeshVisualizer>();
            return trackables
                .Cast<Behaviour>()
                .Append(canvas)
                .ToArray();
        }

        private static string GetFileName() => Application.persistentDataPath + $"/screenshot-{DateTime.Now:s}.png";
    }
}