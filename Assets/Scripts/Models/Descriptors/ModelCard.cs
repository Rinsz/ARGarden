using System.Collections.Generic;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ModelBrowserConstants;
using static UiColorConstants;

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
                favoriteButton.ChangeButtonImageColor(White);
            }
            else
            {
                favorites.Add(id);
                favoriteButton.ChangeButtonImageColor(FavoriteButtonActiveColor);
            }

            PlayerPrefs.SetString(FavoritesKey, string.Join(",", favorites));
        }
    }
}