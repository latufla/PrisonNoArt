using Honeylab.Utils.Data;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapIndicator : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new(0.0f, 8.0f, 0.0f);
        [SerializeField] private Image _icon;

        private ScriptableId _id;
        private Transform _target;
        private Camera _minimapCamera;

        private BillboardPresenter _billboard;


        public ScriptableId Id => _id;


        public void Init(ScriptableId id, Transform target, Camera minimapCamera)
        {
            _id = id;
            _target = target;
            _minimapCamera = minimapCamera;

            Show();
        }


        public void Clear()
        {
            Hide();

            _id = null;
            _target = null;
            _minimapCamera = null;
        }


        public void SetIcon(Sprite icon)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
            }
        }


        public void SetAxis(BillboardAxis axis)
        {
            _billboard.SetAxis(axis);
        }


        private void Show()
        {
            _billboard = new BillboardPresenter(transform, _minimapCamera, BillboardAxis.All);
            _billboard.Run();

            Vector3 position = _target.position + _offset;
            transform.position = position;

            gameObject.SetActive(true);
        }


        private void Hide()
        {
            gameObject.SetActive(false);

            _billboard?.Dispose();
            _billboard = null;
        }
    }
}
