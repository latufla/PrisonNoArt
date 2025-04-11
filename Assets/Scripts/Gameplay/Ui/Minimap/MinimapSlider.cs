using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapSlider : MonoBehaviour
    {
        [SerializeField] private string _firstName;
        [SerializeField] private string _secondName;
        [SerializeField] private Slider _zoomSlider;

        public Slider Slider => _zoomSlider;

        public string FirstName => _firstName;
        public string SecondName => _secondName;

        public IObservable<float> OnZoomSliderChangeAsObserver() => _zoomSlider.onValueChanged.AsObservable();
    }
}
