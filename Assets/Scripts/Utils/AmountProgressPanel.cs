using MPUIKIT;
using TMPro;
using UnityEngine;


namespace Honeylab.Utils
{
    public class AmountProgressPanel : MonoBehaviour
    {
        [SerializeField] private MPImage _progressImage;
        [SerializeField] private AmountPanel _amountPanel;

        private string _progressTextFormat;

        public void SetAmount(int amount, int maxAmount)
        {
            _progressImage.fillAmount = ((float) amount) / maxAmount;
            _amountPanel.SetAmount(amount, maxAmount);
        }
    }
}
