using Cysharp.Threading.Tasks;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class GameObjectsUnlockBuildingAction : UnlockBuildingActionBase
    {
        [SerializeField] private List<UnlockBuildingActionBase> _actions;
        [SerializeField] private List<GameObject> _gameObjects;
        [SerializeField] private bool _activeGameObjectsViseVersa = false;
        [SerializeField] private List<GameObject> _lockedGameObjects;
        [SerializeField] private Transform _cameraFocusTarget;
        [SerializeField] private float _timeBeforeSetActive;
        [SerializeField] private float _unlockTime;
        [SerializeField] private bool _useUnlockVfx;
        [SerializeField] private bool _useUnlockSound;
        [SerializeField] private bool _useUnlockVibration;

      


        protected override void OnInit()
        {
            base.OnInit();

            SetGameObjectsActive(IsUnlocked());
        }


        protected override async UniTask OnUnlockAsync(CancellationToken ct)
        {
            ICameraTargetingHandle handle = null;
            if (_cameraFocusTarget != null)
            {
                handle = Flow.CameraTargeting.Enqueue(_cameraFocusTarget);
                await handle.WaitForFocusAsync(ct);
            }

            PlayVfx();
            PlaySound();
            PlayVibration();

            await UniTask.Delay(TimeSpan.FromSeconds(_timeBeforeSetActive), cancellationToken: ct);
            SetGameObjectsActive(true);

            foreach (UnlockBuildingActionBase action in _actions)
            {
                await action.UnlockAsync(ct);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_unlockTime), cancellationToken: ct);

            if (handle != null)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_unlockTime), cancellationToken: ct);
                handle.Finish();
            }
        }


        private void SetGameObjectsActive(bool isActive)
        {
            _lockedGameObjects.ForEach(it => it.SetActive(!isActive));

            foreach (GameObject go in _gameObjects)
            {
                if (go != null)
                {
                    if (_activeGameObjectsViseVersa)
                        isActive = !isActive;

                    go.SetActive(isActive);
                }
                else
                {
                    throw new Exception("GameObject field in UnlockAction is Empty " + transform.parent.parent.name);
                }
            }
        }


        private void PlayVfx()
        {
            if (_useUnlockVfx)
            {
                View.PlayUnlockVfx();
            }
        }


        private void PlaySound()
        {
            if (_useUnlockSound)
            {
                View.PlayUnlockSound();
            }
        }


        private void PlayVibration()
        {
            if (_useUnlockVibration)
            {
                View.PlayUnlockVibration();
            }
        }
    }
}
