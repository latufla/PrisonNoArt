using TMPro;
using UnityEngine;


namespace Honeylab.Utils
{
    public class AmountPanel : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI _label;

        private string _labelFormat;


        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }


        public void SetAmount(int amount, int maxAmount)
        {
            _labelFormat ??= _label.text;
            _label.text = string.Format(_labelFormat, amount, maxAmount);
        }
    }
}
