using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Tutorial;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;


namespace Honeylab.Gameplay.Tutorial
{
    public enum RequiredItemType
    {
        None,
        Weapon,
        Consumables,
        Craft,
        Upgrade
    }


    public class TutorialScreenRequiredPresenter : MonoBehaviour
    {
        [SerializeField] private TutorialScreenRequiredWeapon _requiredWeapon;
        [SerializeField] private TutorialScreenRequiredConsumables _requiredConsumables;
        [SerializeField] private TutorialScreenRequiredCraft _requiredCraft;
        [SerializeField] private TutorialScreenRequiredUpgrade _requiredUpgrade;

        private TutorialScreenRequiredItem _currentRequiredItem;
        private CancellationTokenSource _cts;
        private WorldObjectsService _world;

        public bool RequiredIsActive => _currentRequiredItem != null;
        public bool RequiredIsCompleted => RequiredIsActive && _currentRequiredItem.IsCompleted();


        public void Init(TutorialFlow flow,
            TutorialScreenFocus screenFocus)
        {
            _world = flow.Resolve<WorldObjectsService>();
            _requiredWeapon.Init(flow, screenFocus);
            _requiredConsumables.Init(flow, screenFocus);
            _requiredCraft.Init(flow, screenFocus);
            _requiredUpgrade.Init(flow, screenFocus);
        }


        public void Run(TutorialInfo tutorialInfo)
        {
            _cts = new CancellationTokenSource();
            RunAsync(tutorialInfo, _cts.Token).Forget();
        }

        private async UniTask RunAsync(TutorialInfo tutorialInfo, CancellationToken ct)
        {
            await UniTask.WaitUntil(() =>
            {
                PlayerFlow playerFlow = _world.GetObjects<PlayerFlow>().First();
                return playerFlow != null;
            },
            cancellationToken: ct);

            switch (tutorialInfo.RequiredItemType)
            {
                case RequiredItemType.None:
                {
                    return;
                }
                case RequiredItemType.Weapon:
                {
                    _currentRequiredItem = _requiredWeapon;
                    break;
                }
                case RequiredItemType.Consumables:
                {
                    _currentRequiredItem = _requiredConsumables;
                    break;
                }
                case RequiredItemType.Craft:
                {
                    _currentRequiredItem = _requiredCraft;
                    break;
                }
                case RequiredItemType.Upgrade:
                {
                    _currentRequiredItem = _requiredUpgrade;
                    break;
                }
            }

            _currentRequiredItem.Run(tutorialInfo);

            RequiredItemsButtonActive(true);
        }

        public void RequiredItemsButtonActive(bool active)
        {
            if (_currentRequiredItem == null)
            {
                return;
            }

            _currentRequiredItem.Button.interactable = active;
        }


        public void Hide()
        {
            if (_currentRequiredItem == null)
            {
                return;
            }

            _currentRequiredItem.Root.SetActive(false);
            RequiredItemsButtonActive(false);
            _currentRequiredItem.Hide();
            _currentRequiredItem = null;

            _cts.CancelThenDispose();
            _cts = null;
        }
    }
}
