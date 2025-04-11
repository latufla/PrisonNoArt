using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class UiWorldItemConsumablesInfo : UiWorldItemInfo
    {
        [SerializeField] private List<Image> _icons = new List<Image>();


        public void AddConsumableIcons(List<Sprite> sprites)
        {
            for (int i = 0; i < _icons.Count; i++)
            {
                if (sprites.Count > i)
                {
                    _icons[i].sprite = sprites[i];
                    _icons[i].gameObject.SetActive(true);
                    continue;
                }
                _icons[i].gameObject.SetActive(false);
            }
        }
    }
}
