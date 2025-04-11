using MPUIKIT;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class ProgressPopup : PopupBase
    {
        [SerializeField] private MPImage _progressFillImage;


        public void SetProgress(float progress) => _progressFillImage.fillAmount = progress;
    }
}
