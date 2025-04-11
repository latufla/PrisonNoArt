using UnityEngine;


namespace Honeylab.Utils
{
    public class CircleStrokeAreaView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _renderer;


        public void UpdateView(Vector3 origin, float radius, int steps)
        {
            DrawCircle(origin, radius, steps);
        }


        private void DrawCircle(Vector3 origin, float radius, int steps)
        {
            transform.position = origin;

            _renderer.loop = true;
            _renderer.positionCount = steps;

            float angle = 0f;

            for (int i = 0; i < steps; i++)
            {
                float x = radius * Mathf.Cos(angle);
                float y = radius * Mathf.Sin(angle);

                _renderer.SetPosition(i, new Vector3(x, y, 0f));

                angle += 2f * Mathf.PI / steps;
            }
        }
    }
}
