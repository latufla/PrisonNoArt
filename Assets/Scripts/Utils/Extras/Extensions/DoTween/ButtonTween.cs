using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Honeylab.Utils.Extensions
{
    [RequireComponent(typeof(Button))]
    public class ButtonTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 _localScaleBackup;


        private void Start()
        {
            _localScaleBackup = transform.localScale;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            DOTween.Kill(transform);
            Vector3 newScale = 1.2f * _localScaleBackup;
            transform.DOScale(newScale, 0.1f)
                .SetLink(gameObject)
                .SetEase(Ease.InQuad);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            DOTween.Kill(transform);
            transform.DOScale(_localScaleBackup, 0.15f)
                .SetLink(gameObject)
                .SetEase(Ease.OutQuad);
        }
    }
}
