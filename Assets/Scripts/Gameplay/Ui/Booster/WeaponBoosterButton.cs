using Honeylab.Gameplay.Ui.Ads;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Booster
{
    public class WeaponBoosterButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _layout;
        [SerializeField] private RewardedAdButton _rvButton;
        [SerializeField] private WeaponBoosterFreeButton _freeButton;

        public RewardedAdButton RvButton => _rvButton;
        public WeaponBoosterFreeButton FreeButton => _freeButton;

        public void SetActive(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
        }
    }
}
