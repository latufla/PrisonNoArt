using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Interactables.World;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Ui.Minimap;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Honeylab.Gameplay.Tutorial
{
    public class MoveInteractableStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _worldObjectId;
        [SerializeField] private ConsumablePersistenceId _craftConsumableId;
        [SerializeField] private WorldObjectId _unlockWorldId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private float _endDelay = 1.0f;
        private ConsumablesService _consumablesService;

        [Inject]
        public void Construct(ConsumablesService consumablesService)
        {
            _consumablesService = consumablesService;
        }

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await MoveInteractableAsync(ct)
        };


        private async UniTask MoveInteractableAsync(CancellationToken ct)
        {
            SendStepStartAnalytics(_info);
            if (CheckConsumable())
            {
                //ShowScreen(_info);
                return;
            }

            WorldObjectFlow building = null;
            await UniTask.WaitUntil(() =>
                {
                    building = GetObject<WorldObjectFlow>(_worldObjectId);
                    return building != null;
                },
                cancellationToken: ct);

            Transform target = building.transform;
            ShowScreen(_info, target);


            IArrowHandle arrow = ShowTargetArrow(target, _info.ArrowPositionY);
            ShowOffscreenIndicator(target, _info.Icon);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            InteractableBase interactable = building.Get<InteractableBase>();
            PlayerFlow playerFlow = await GetObjectFirstAsync<PlayerFlow>(ct);

            List<UniTask> asyncList;
            if (interactable != null)
            {
                asyncList = new()
                {
                    interactable.OnInteractAsObservable().ToUniTask(true, ct),
                    UniTask.WaitUntil(CheckConsumable, cancellationToken: ct)
                };
            }
            else
            {
                asyncList = new()
                {
                    UniTask.WaitUntil(() => Vector3.Distance(target.position, playerFlow.transform.position) < 5,
                        cancellationToken: ct),
                    UniTask.WaitUntil(CheckConsumable, cancellationToken: ct)
                };
            }

            await UniTask.WhenAny(asyncList);

            HideTargetArrow(arrow);
            HideOffscreenIndicator();

            await UniTask.Delay(TimeSpan.FromSeconds(_endDelay), cancellationToken: ct);
        }

        private bool CheckConsumable()
        {
            var craftAmountValue = _craftConsumableId != null ? _consumablesService.GetAmountProp(_craftConsumableId).Value : 0;
            bool isUnlocked = false;
            if (_unlockWorldId != null)
            {
                var building = GetObject<UnlockBuildingFlow>(_unlockWorldId);
                if (building != null && building.State.Value == UnlockBuildingStates.Unlocked)
                {
                    isUnlocked = true;
                }
            }
            return craftAmountValue > 0 || isUnlocked;
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
