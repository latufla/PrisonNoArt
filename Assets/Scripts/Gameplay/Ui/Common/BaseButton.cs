using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Ui
{
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] private string _name;
        [SerializeField] private Button _button;

        public IObservable<Unit> OnButtonClickAsObservable() => _button.OnClickAsObservable();

        public Button GetButton => _button;
        public string Name => _name;

        public virtual void SetActive(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
        }

        public bool IsActive() => _button.gameObject.activeInHierarchy;
    }
}
