using Honeylab.Consumables;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ConsumableFlyToast : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private FlyToastAnimation _animation;


        public void SetIconSprite(Sprite sprite) => _iconImage.sprite = sprite;


        public void Show(ConsumableCounterView screenConsumable,
            Transform parent,
            Vector3 toastEndPosition,
            Action callback) => _animation.Show(screenConsumable, parent, toastEndPosition, callback);

        public void Show(Transform target,
            Transform parent,
            Vector3 toastEndPosition,
            Action callback) => _animation.Show(target, parent, toastEndPosition, callback);
    }
}
