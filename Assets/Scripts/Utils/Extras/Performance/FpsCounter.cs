using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Utils.Performance
{
    public class FpsCounter : MonoBehaviour
    {
        private Text _fpsText;

        private float _timer;
        private float _hudRefreshRate = 1f;


        private void Awake()
        {
            _fpsText = GetComponent<Text>();
        }


        private void Update()
        {
            if (Time.unscaledTime > _timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                _fpsText.text = fps.ToString();
                _timer = Time.unscaledTime + _hudRefreshRate;
            }
        }
    }
}
