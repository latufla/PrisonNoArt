using MPUIKIT;
using UnityEngine;


namespace Honeylab.Utils
{
    public class TimeProgressPanel : MonoBehaviour
    {
        [SerializeField] private MPImage _progressImage;
        [SerializeField] private TimePanel _progressTimePanel;


        public void SetTime(float timeLeft, float duration, bool inverted = false)
        {
            float progress = inverted ? timeLeft / duration : 1.0f - timeLeft / duration;
            _progressImage.fillAmount = progress;
            _progressTimePanel.SetTime(timeLeft);
        }
    }
}
