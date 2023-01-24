using System;
using System.Linq;
using Lean.Touch;
using UnityEngine;

namespace Models
{
    public class LeanFingerMarker : MonoBehaviour
    {
        private GameObject[] createdMarkers = Array.Empty<GameObject>();
        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private Transform canvas;

        public LeanFingerFilter Use = new(true);

        protected virtual void Awake()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }

        protected virtual void Update()
        {
#if !UNITY_EDITOR
            return
#endif

            var fingers = Use.UpdateAndGetFingers();
            if (createdMarkers.Length < fingers.Count)
            {
                createdMarkers = createdMarkers
                    .Concat(Enumerable
                        .Range(0, fingers.Count - createdMarkers.Length)
                        .Select(_ => Instantiate(markerPrefab, canvas)))
                    .ToArray();
            }

            for (var i = 0; i < fingers.Count; i++)
            {
                createdMarkers[i].SetActive(true);
                ((RectTransform)createdMarkers[i].transform).anchoredPosition = fingers[i].ScreenPosition - new Vector2(Screen.width / 2, Screen.height / 2);
            }
            for (var i = fingers.Count; i < createdMarkers.Length; i++)
                createdMarkers[i].SetActive(false);
        }
    }
}