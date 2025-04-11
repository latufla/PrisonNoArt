using UnityEngine;


namespace Honeylab.Utils.Vfx
{
    public class Vfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _root;
        [SerializeField] private float _duration;

        public float Duration => _duration;


        public void Play()
        {
            _root.Stop();
            _root.Play();
        }


        public void Stop()
        {
            _root.Stop();
        }
    }
}
