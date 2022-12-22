using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadBar : MonoBehaviour
    {
        public Slider barGraphic;
        public TMP_Text loadText;

        public void RunLoader(AsyncOperation asyncOperation)
        {
            if (this.gameObject.activeSelf)
                return;

            this.gameObject.SetActive(true);
            StartCoroutine(ProcessDownload(asyncOperation).GetEnumerator());
        }

        private IEnumerable ProcessDownload(AsyncOperation asyncOperation)
        {
            while (!asyncOperation.isDone)
            {
                barGraphic.value = asyncOperation.progress;
                loadText.text = $"{asyncOperation.progress:P}";
                yield return this;
            }

            this.gameObject.SetActive(false);
        }
    }
}
