using System;
using System.Collections.Generic;
using Honeylab.Consumables;
using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Pools;
using Honeylab.Pools;
using Honeylab.Utils.Pool;
using UniRx;
using UnityEngine;

namespace Honeylab.Gameplay.Ui
{
    public class ToastsService : IDisposable
    {
        private readonly CompositeDisposable _disposable = new();

        private readonly Dictionary<IGameObjectPool, Subject<GameObject>> _toastPushSubjects = new();

        private readonly IGameObjectPool _consumableToastsPool;
        private readonly IGameObjectPool _consumableFlyToastsPool;
        private readonly IGameObjectPool _textToastsPool;
        private readonly IGameObjectPool _damageToastsPool;
        private readonly IGameObjectPool _speechToastsPool;
        private readonly Transform _toastsParent;
        private readonly ConsumablesData _consumablesData;
        private readonly Transform _gameCameraTransform;
        private readonly GameplayScreen _gameplayScreen;


        public ToastsService(GameplayPoolsService pools,
            Transform toastsParent,
            ConsumablesData consumablesData,
            CameraProvider cameraProvider,
            GameplayScreen gameplayScreen)
        {
            _consumableToastsPool = pools.Get<ConsumableToastsPool>();
            _consumableFlyToastsPool = pools.Get<ConsumableFlyToastsPool>();
            _textToastsPool = pools.Get<TextToastsPool>();
            _damageToastsPool = pools.Get<DamageToastsPool>();
            _speechToastsPool = pools.Get<SpeechToastsPool>();
            _toastsParent = toastsParent;
            _consumablesData = consumablesData;
            _gameCameraTransform = cameraProvider.GameCamera.transform;
            _gameplayScreen = gameplayScreen;
        }


        public void Run()
        {
            var allToastPools = new[]
            {
                _consumableToastsPool, _consumableFlyToastsPool, _textToastsPool, _speechToastsPool, _damageToastsPool
            };
            foreach (IGameObjectPool pool in allToastPools)
            {
                var subject = new Subject<GameObject>();
                _toastPushSubjects[pool] = subject;
                subject.Delay(TimeSpan.FromSeconds(2.0f))
                    .Subscribe(pool.Push)
                    .AddTo(_disposable);
            }
        }


        public void ShowConsumableToast(ConsumableToastArgs args)
        {
            Sprite sprite = _consumablesData.GetData(args.PersistenceId).Sprite;
            int amount = args.Amount;
            Vector3 worldPosition = args.WorldPoint;
            ShowIconToast(sprite, amount, worldPosition);
        }


        public void ShowIconToast(Sprite icon, int amount, Vector3 worldPoint)
        {
            ConsumableToast toast = _consumableToastsPool.PopWithComponent<ConsumableToast>(true);
            PlaceToast(toast.transform, worldPoint);

            toast.SetIconSprite(icon);
            toast.SetAmount(amount);
            toast.Show();
            _toastPushSubjects[_consumableToastsPool].OnNext(toast.gameObject);
        }


        public void ShowConsumableFlyToast(ConsumablePersistenceId id,
            Vector3 toastPosition,
            Vector3 toastEndPosition,
            Action callback = default)
        {
            ConsumableType type = _consumablesData.GetData(id).ConsumableType;
            if (type is ConsumableType.Hidden)
            {
                return;
            }

            ConsumableFlyToast toast = _consumableFlyToastsPool.PopWithComponent<ConsumableFlyToast>(true);
            PlaceToast(toast.transform, toastPosition);
            toast.SetIconSprite(_consumablesData.GetData(id).Sprite);

            var screenConsumable = _gameplayScreen.GetConsumableCounter(id);
            var parent = _gameplayScreen.ConsumableCountersRoot;

            toast.Show(screenConsumable,
                parent,
                toastEndPosition,
                () =>
                {
                    _toastPushSubjects[_consumableFlyToastsPool].OnNext(toast.gameObject);
                    callback?.Invoke();
                });
        }


        public void ShowConsumableFlyToast(ConsumablePersistenceId id,
            Transform target,
            Vector3 toastPosition,
            Vector3 toastEndPosition)
        {
            ConsumableFlyToast toast = _consumableFlyToastsPool.PopWithComponent<ConsumableFlyToast>(true);
            PlaceToast(toast.transform, toastPosition);
            toast.SetIconSprite(_consumablesData.GetData(id).Sprite);

            toast.Show(target,
                target.parent,
                toastEndPosition,
                () => _toastPushSubjects[_consumableFlyToastsPool].OnNext(toast.gameObject));
        }


        public TextToast ShowTextToast(Vector3 position, string text)
        {
            TextToast toast = _textToastsPool.PopWithComponent<TextToast>(true);
            PlaceToast(toast.transform, position);

            toast.SetText(text);
            toast.Show();
            _toastPushSubjects[_textToastsPool].OnNext(toast.gameObject);

            return toast;
        }


        public TextToast ShowDamageToast(Vector3 position, string text)
        {
            TextToast toast = _damageToastsPool.PopWithComponent<TextToast>(true);
            PlaceToast(toast.transform, position);

            toast.SetText(text);
            toast.Show();
            _toastPushSubjects[_damageToastsPool].OnNext(toast.gameObject);

            return toast;
        }


        public void ShowSpeechToast(Vector3 position, string text, float time)
        {
            SpeechToast toast = _speechToastsPool.PopWithComponent<SpeechToast>(true);
            PlaceToast(toast.transform, position);

            toast.Show(time, () => _toastPushSubjects[_speechToastsPool].OnNext(toast.gameObject));
            toast.SetText(text);
        }


        public void Dispose() => _disposable.Dispose();


        private void PlaceToast(Transform toastTransform, Vector3 position)
        {
            toastTransform.parent = _toastsParent;
            toastTransform.position = position;
            toastTransform.rotation = _gameCameraTransform.rotation;
        }
    }
}
