using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Honeylab.SeaPort.Ui
{
    [RequireComponent(typeof(Button))]
    public class ButtonTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 _localScaleBackup;
        private Button _button;

        private void Start()
        {
            _localScaleBackup = transform.localScale;
            _button = GetComponent<Button>();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            DOTween.Kill(transform);
            Vector3 newScale = 1.2f * _localScaleBackup;
            transform.DOScale(newScale, 0.1f)
                .SetUpdate(true)
                .SetLink(gameObject)
                .SetEase(Ease.InQuad);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            DOTween.Kill(transform);
            transform.DOScale(_localScaleBackup, 0.15f)
                .SetUpdate(true)
                .SetLink(gameObject)
                .SetEase(Ease.OutQuad);
        }
    }
}
