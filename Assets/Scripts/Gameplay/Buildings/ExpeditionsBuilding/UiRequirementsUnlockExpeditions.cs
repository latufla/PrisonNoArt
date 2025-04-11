using TMPro;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class UiRequirementsUnlockExpeditions : UiWorldItemInfo
    {
        [SerializeField] private TextMeshProUGUI _textLabel;
        [SerializeField] private GameObject _checkMark;


        public void CheckMarkActive(bool active)
        {
            _checkMark.SetActive(active);
        }


        public void SetText(string text)
        {
            _textLabel.text = text;
        }
    }
}
