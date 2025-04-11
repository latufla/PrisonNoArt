using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Ui
{
    public class TutorialScreenFocusButton : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Button _button;

        public IObservable<Unit> OnButtonClickAsObservable() => _button.OnClickAsObservable();
        public Button Button => _button;
        public string Name => _name;

        public void SetActive(bool isEnabled)
        {
            _button.gameObject.SetActive(isEnabled);
        }
    }
}
