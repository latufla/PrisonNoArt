using Honeylab.Gameplay.World;
using Honeylab.Project;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Utils
{
    public class RadialAreaView : SearchAreaView
    {
        [SerializeField] private RectTransform _canvasTransform;
        [SerializeField] private Image _fillImage;
        [SerializeField] private RectTransform _fillTransform;


        public void Init(WorldObjectFlow flow)
        {
            ProjectConfig projectConfig = flow.Resolve<ProjectConfig>();
            SetActive(projectConfig.DebugAreasEnabled);
        }


        public void UpdateView(Vector3 origin, float radius)
        {
            transform.position = origin;

            Vector3 canvasLocalScale = _canvasTransform.localScale;
            Vector2 pixelsPerMeter = new(1.0f / canvasLocalScale.x, 1.0f / canvasLocalScale.y);
            Vector2 newSize = 2.0f * radius * pixelsPerMeter;
            _canvasTransform.sizeDelta = newSize;

            _fillImage.fillAmount = 1.0f;
        }


        public void UpdateView(Vector3 origin, Vector3 direction, float radius, float angle)
        {
            transform.position = origin;

            Quaternion rotation = Quaternion.LookRotation(direction);
            Vector3 rotationEuler = new(0.0f, rotation.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Euler(rotationEuler);

            Vector3 canvasLocalScale = _canvasTransform.localScale;
            Vector2 pixelsPerMeter = new(1.0f / canvasLocalScale.x, 1.0f / canvasLocalScale.y);
            Vector2 newSize = 2.0f * radius * pixelsPerMeter;
            _canvasTransform.sizeDelta = newSize;

            _fillTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle / 2.0f);
            _fillImage.fillAmount = angle / 360.0f;
        }


        private void SetActive(bool isActive) => gameObject.SetActive(isActive);
    }
}
