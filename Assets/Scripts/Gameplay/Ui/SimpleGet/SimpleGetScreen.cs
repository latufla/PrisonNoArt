using Cysharp.Threading.Tasks;
using Honeylab.Ads;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Ui.Ads;
using Honeylab.Gameplay.Ui.Consumables;
using Honeylab.Pools;
using Honeylab.Utils;
using Honeylab.Utils.Data;
using Honeylab.Utils.Pool;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class SimpleGetScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private GameObject _titleTextContent;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _rewardedAdText;
        [SerializeField] private Image _rewardedImage;
        [SerializeField] private RewardedAdButton _rewardedAdButton;
        [SerializeField] private TextMeshProUGUI _consuamblesText;
        [SerializeField] private Image _consumablesImage;
        [SerializeField] private ConsumablesButton _consumablesButton;
        [SerializeField] private ConsumableCounterView _consumableCounterView;

        private string _rewardedAdTextFormat;
        private string _consuamblesTextFormat;
        private ItemFlyersPool _itemFlyersPool;

        public Vector3 RewardedImagePosition => _rewardedImage.transform.position;
        public Vector3 ConsumablesImagePosition => _consumablesImage.transform.position;

        public override string Name => ScreenName.GetScreen;


        public IObservable<RewardedAdResult> OnRewardedAdShownAsObservable() =>
            _rewardedAdButton.OnRewardedAdShownAsObservable();


        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public ConsumablesButton ConsumablesButton => _consumablesButton;


        public void Init(ItemFlyersPool itemFlyersPool)
        {
            _itemFlyersPool = itemFlyersPool;
        }


        public void Run(RewardedAdsService rewardedAdsService, ScriptableId id, string location, string titleText)
        {
            _rewardedAdButton.Init(rewardedAdsService, id, location);
            _rewardedAdButton.Run();

            TitleTextActive(titleText);
        }


        private void TitleTextActive(string text)
        {
            _titleText.text = text;
            _titleTextContent.SetActive(text != string.Empty);
        }


        public void SetInfoForRv(int amount, Sprite sprite)
        {
            _rewardedAdTextFormat ??= _rewardedAdText.text;
            _rewardedAdText.text = string.Format(_rewardedAdTextFormat, amount);
            _rewardedImage.sprite = sprite;
        }


        public void SetInfoForConsumables(int amount, int price, Sprite sprite, Sprite spritePrice, bool isActive)
        {
            _consuamblesTextFormat ??= _consuamblesText.text;
            _consuamblesText.text = string.Format(_consuamblesTextFormat, amount);
            _consumablesImage.sprite = sprite;
            _consumablesButton.SetEnabled(isActive);
            _consumablesButton.SetAmount(price);
            _consumablesButton.SetIcon(spritePrice);
        }


        public async UniTask SetConsumableCounterInfo(int amountResult,
            int amount,
            Sprite sprite,
            Vector3 startPos,
            CancellationToken ct)
        {
            if (amount > 0)
            {
                await RunItemFlyerAsync(amount, sprite, startPos, ct);
            }

            _consumableCounterView.SetAmount(amountResult);
            _consumableCounterView.SetIcon(sprite);
        }


        private async UniTask RunItemFlyerAsync(int amount, Sprite sprite, Vector3 startPos, CancellationToken ct)
        {
            ItemsFlyer itemsFlyer = _itemFlyersPool.PopWithComponent<ItemsFlyer>(true);
            itemsFlyer.transform.SetParent(transform);
            itemsFlyer.transform.position = startPos;
            itemsFlyer.transform.localScale = Vector3.one;

            itemsFlyer.Init();

            await itemsFlyer.RunAsync(sprite, amount, _consumableCounterView.transform.position, ct);
            itemsFlyer.Clear();

            _itemFlyersPool.Push(itemsFlyer.gameObject);
        }


        public void Stop()
        {
            _rewardedAdButton.Clear();
        }
    }
}
