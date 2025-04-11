using Honeylab.Gameplay.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace Honeylab.Utils
{
    public class SlidersPlayerInputBlock : MonoBehaviour
    {
        [SerializeField] private List<Slider> _sliders;

        private PlayerInputService _input;

        private readonly List<IDisposable> _blocks = new();
        private CompositeDisposable _disposable;


        [Inject]
        public void Construct(PlayerInputService input)
        {
            _input = input;
        }


        private void OnEnable()
        {
            _disposable = new();

            foreach (Slider slider in _sliders)
            {
                IDisposable sliderDown = slider.OnPointerDownAsObservable()
                    .Subscribe(_ => { _blocks.Add(_input.BlockInput()); });
                _disposable.Add(sliderDown);

                IDisposable sliderUp = slider.OnPointerUpAsObservable()
                    .Subscribe(_ =>
                    {
                        IDisposable block = _blocks.Last();
                        block.Dispose();

                        _blocks.Remove(block);
                    });
                _disposable.Add(sliderUp);
            }
        }


        private void OnDisable()
        {
            _blocks.ForEach(it => it.Dispose());
            _blocks.Clear();

            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
