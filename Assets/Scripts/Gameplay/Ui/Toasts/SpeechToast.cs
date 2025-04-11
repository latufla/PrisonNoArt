using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class SpeechToast : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private SpeechToastAnimation _toastAnimation;

        public void Show(float time, Action callback)
        {
            _toastAnimation.Show(time, callback);
        }


        public void SetText(string text)
        {
            _label.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_label.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        }
    }
}
