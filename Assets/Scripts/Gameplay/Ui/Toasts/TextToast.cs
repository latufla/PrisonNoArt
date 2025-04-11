using TMPro;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class TextToast : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private ToastAnimation _animation;


        public void Show() => _animation.Show();
        public void SetText(string text) => _label.text = text;
    }
}
