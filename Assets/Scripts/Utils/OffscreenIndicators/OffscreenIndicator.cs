using Honeylab.Utils.Data;
using PixelPlay.OffScreenIndicator;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Utils.OffscreenTargetIndicators
{
    public class OffscreenIndicator : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Transform _rotationRoot;
        [SerializeField] private float _offset = 0.8f;

        private ScriptableId _id;
        private Transform _target;
        private bool _isExpeditionEnemy;
        private bool _isEnemy;

        private Camera _mainCamera;
        private Vector3 _screenCenter;
        private Vector3 _screenBounds;
        private bool _isVisible = true;


        public ScriptableId Id => _id;


        public void Init(ScriptableId id, Transform target, bool isExpeditionEnemy = false)
        {
            _id = id;
            _target = target;
            _isExpeditionEnemy = isExpeditionEnemy;

            _mainCamera = Camera.main;
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            _screenBounds = _screenCenter * _offset;
        }


        public void Clear()
        {
            if (_icon != null)
            {
                _icon.gameObject.SetActive(false);
            }

            _id = null;
            _target = null;
        }


        public void UpdateIndicator(Transform player, float distance)
        {
            Vector3 screenPosition = OffScreenIndicatorCore.GetScreenPosition(_mainCamera, _target.position);
            bool isTargetVisible = OffScreenIndicatorCore.IsTargetVisible(screenPosition);
            if (isTargetVisible)
            {
                gameObject.SetActive(false);
                return;
            }

            if (_isExpeditionEnemy && Vector3.Distance(player.position, _target.position) > distance)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(_isVisible);

            float angle = float.MinValue;
            OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPosition,
                ref angle,
                _screenCenter,
                _screenBounds);

            transform.position = screenPosition;
            _rotationRoot.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }


        public void SetVisible(bool isVisible)
        {
            _isVisible = isVisible;
        }


        public void SetIcon(Sprite icon)
        {
            bool hasIcon = _icon != null;
            if (hasIcon)
            {
                _icon.sprite = icon;
            }
            _icon.gameObject.SetActive(hasIcon);
        }
    }
}
