using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.AdOffer
{
    public class AdOfferScreenRewardPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _button;
        [SerializeField] private Transform _consumableItemsStorage;
        private GameObjectPoolBehaviour _pool;
        private ConsumablesData _consumablesData;
        private readonly List<UiConsumableItemInfo> _itemInfos = new List<UiConsumableItemInfo>();


        public IObservable<Unit> OnClickButtonAsObservable() =>
            _button.OnClickAsObservable().Merge(_backgroundButton.OnClickAsObservable());


        public void Init(GameObjectPoolBehaviour pool, ConsumablesData consumablesData)
        {
            _pool = pool;
            _consumablesData = consumablesData;
        }


        public void SetInfo(RewardAmountConfig reward)
        {
            _root.SetActive(true);
            SetRewards(reward);
        }


        private void SetRewards(RewardAmountConfig reward)
        {
            SetRewards(new List<RewardAmountConfig>
            {
                new()
                {
                    Name = reward.Name,
                    Amount = reward.Amount
                }
            });
        }


        private void SetRewards(List<RewardAmountConfig> rewards)
        {
            foreach (RewardAmountConfig reward in rewards)
            {
                GameObject consumable = _pool.Pop(true);

                if (consumable.TryGetComponent(out UiConsumableItemInfo itemInfo))
                {
                    itemInfo.transform.SetParent(_consumableItemsStorage);
                    itemInfo.transform.localScale = Vector3.one;
                }
                else
                {
                    _pool.Push(consumable);
                    continue;
                }

                ConsumableData data = _consumablesData.GetData(reward.Name);

                itemInfo.SetAmount(reward.Amount);
                itemInfo.SetIcon(data.Sprite);

                _itemInfos.Add(itemInfo);
            }
        }


        public void Clear()
        {
            _root.SetActive(false);
            foreach (UiConsumableItemInfo item in _itemInfos)
            {
                _pool.Push(item.gameObject);
            }

            _itemInfos.Clear();
        }
    }
}
