using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using System;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private MinimapSlider _zoomSlider;
        [SerializeField] private float _zoomIn = 13;
        [SerializeField] private float _zoomOut = 20;
        [SerializeField] private RawImage _minimapImage;
        [SerializeField] private float _dragMultiplier = 0.25f;
        [SerializeField] private Animator _handAnimator;
        [SerializeField] private TextMeshProUGUI _tutorialText;

        private CameraProvider _cameraProvider;
        private WorldObjectsService _world;
        private MinimapIndicatorsService _minimapIndicatorsService;
        private bool _zoomSliderIsReady;

        private CompositeDisposable _runDisposable;
        private PlayerFlow _player;

        public MinimapSlider MinimapZoomSlider => _zoomSlider;

        public IObservable<PointerEventData> OnDragAsObservable() => _minimapImage.OnDragAsObservable();
        
        public bool ZoomSliderIsReady => _zoomSliderIsReady;

        public override string Name => ScreenName.Minimap;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Run(CameraProvider cameraProvider,
            WorldObjectsService world,
            MinimapIndicatorsService minimapIndicatorsService)
        {
            _cameraProvider = cameraProvider;
            _world = world;
            _minimapIndicatorsService = minimapIndicatorsService;
            _player = _world.GetObjects<PlayerFlow>().FirstOrDefault();

            if (_player == null)
            {
                _backgroundButton.onClick.Invoke();
                return;
            }

            _zoomSlider.Slider.minValue = 0.0f;
            _zoomSlider.Slider.maxValue = 1.0f;
            _zoomSlider.Slider.value = 0.5f;

            _runDisposable?.Dispose();
            _runDisposable = new();

            _cameraProvider.MinimapCamera.gameObject.SetActive(true);

            Vector3 position = _player.transform.position;
            UpdateCameraPosition(position.x, position.z);

            _cameraProvider.MinimapCamera.orthographicSize = CalcZoom();

            ShowIndicators();

            IDisposable zoomSliderChange = _zoomSlider.OnZoomSliderChangeAsObserver()
                .Subscribe(_ => { _cameraProvider.MinimapCamera.orthographicSize = CalcZoom(); });
            _runDisposable.Add(zoomSliderChange);

            IDisposable minimapDrag = _minimapImage.OnDragAsObservable()
                .Subscribe(it =>
                {
                    Vector3 pos = _cameraProvider.MinimapCamera.transform.position;
                    Vector2 delta = it.delta * _dragMultiplier / (1.0f + _zoomSlider.Slider.value);
                    UpdateCameraPosition(pos.x + delta.y, pos.z - delta.x);
                });
            _runDisposable.Add(minimapDrag);
            _zoomSliderIsReady = true;
        }


        public void Stop()
        {
            HideIndicators();

            _cameraProvider.MinimapCamera.gameObject.SetActive(false);

            _runDisposable?.Dispose();
            _runDisposable = null;

            _zoomSliderIsReady = false;
        }


        private void ShowIndicators()
        {
            var targets = _world.GetObjects()
                .Select(it => it.Get<MinimapIndicatorTarget>())
                .Where(t => t != null);

            foreach (MinimapIndicatorTarget t in targets)
            {
                MinimapIndicator indicator = _minimapIndicatorsService.Add(t.MinimapIndicatorId, t.transform);
                t.UpdateIndicator(indicator);
            }
        }


        private void HideIndicators()
        {
            _minimapIndicatorsService.RemoveAll();
        }


        private float CalcZoom() => _zoomIn + (_zoomSlider.Slider.maxValue - _zoomSlider.Slider.value) * (_zoomOut - _zoomIn);


        private void UpdateCameraPosition(float x, float z)
        {
            Transform cameraTransform = _cameraProvider.MinimapCamera.gameObject.transform;
            Vector3 position = cameraTransform.position;

            if (_cameraProvider.MinimapCameraBounds.CheckBounds(x, z))
            {
                position.x = x;
                position.z = z;
            }
            else
            {
                Vector2 closestPoint = _cameraProvider.MinimapCameraBounds.ClosestPointOnBounds(x, z);
                position.x = closestPoint.x;
                position.z = closestPoint.y;
            }

            cameraTransform.position = position;
        }


        public void PlayHandAnimation(bool play)
        {
            if (play)
            {
                _handAnimator.SetTrigger("Drag");
            }
            else
            {
                _handAnimator.SetTrigger("Idle");
            }
        }


        public void TutorialSetText(string text)
        {
            _tutorialText.gameObject.SetActive(text != string.Empty);
            _tutorialText.text = text;
        }
    }
}
