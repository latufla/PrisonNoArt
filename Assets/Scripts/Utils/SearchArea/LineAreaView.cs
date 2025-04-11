using UnityEngine;


namespace Honeylab.Utils
{
    public class LineAreaView : SearchAreaView
    {
        [SerializeField] private RectTransform _canvasTransform;


        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void UpdateView(Transform origin, Vector3 direction, float radius, float width)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 rotationEuler = new(0.0f, rotation.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Euler(rotationEuler);
            transform.position = origin.transform.TransformPoint(Vector3.forward * radius);

            Vector3 canvasLocalScale = _canvasTransform.localScale;
            Vector2 pixelsPerMeter = new(1.0f / canvasLocalScale.x * width, 1.0f / canvasLocalScale.y * radius);
            Vector2 newSize = 2.0f * pixelsPerMeter;
            _canvasTransform.sizeDelta = newSize;
        }
    }
}
