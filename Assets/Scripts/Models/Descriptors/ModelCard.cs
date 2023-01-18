using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ModelBrowserConstants;

namespace Models.Descriptors
{
    [RequireComponent(typeof(Button))]
    public class ModelCard : MonoBehaviour
    {
        public Button selectButton;
        public Button favoriteButton;
        public Image modelIcon;
        public TMP_Text modelName;
        [HideInInspector] public ModelMeta meta;

        public void Favorite(ref HashSet<string> favorites)
        {
            var id = this.meta.Id.ToString();
            if (favorites.Contains(id))
            {
                favorites.Remove(id);
                favoriteButton.image.color = new Color(255, 255, 255);
            }
            else
            {
                favorites.Add(id);
                favoriteButton.image.color = new Color(122, 55, 33);
            }

            PlayerPrefs.SetString(FavoritesKey, string.Join(",", favorites));
        }
    }
}