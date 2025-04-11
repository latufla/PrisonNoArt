using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Player
{
    public class PlayerInputInstaller : MonoInstaller
    {
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private RectTransform _inputRect;
        [SerializeField] private PlayerInputView _inputView;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerInputService>()
                .AsSingle()
                .WithArguments(_inputRect, _uiCamera);

            Container.BindInterfacesAndSelfTo<PlayerInputPresenter>()
                .AsSingle()
                .WithArguments(_inputView);
        }
    }
}
