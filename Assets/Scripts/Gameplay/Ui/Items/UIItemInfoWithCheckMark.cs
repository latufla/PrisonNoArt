using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class UIItemInfoWithCheckMark : UiConsumableItemInfo
    {
        [SerializeField] private GameObject _checkMark;

        public void CheckMarkEnabled(bool active)
        {
            _checkMark.SetActive(active);
        }
    }
}
