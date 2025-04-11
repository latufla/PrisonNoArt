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
    public class ButtonsPlayerInputBlock : MonoBehaviour
    {
        [SerializeField] private List<Button> _buttons;

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

            foreach (Button button in _buttons)
            {
                IDisposable buttonDown = button.OnPointerDownAsObservable()
                    .Subscribe(_ => { _blocks.Add(_input.BlockInput()); });
                _disposable.Add(buttonDown);

                IDisposable buttonUp = button.OnPointerUpAsObservable()
                    .Subscribe(_ =>
                    {
                        IDisposable block = _blocks.Last();
                        block.Dispose();

                        _blocks.Remove(block);
                    });
                _disposable.Add(buttonUp);
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
