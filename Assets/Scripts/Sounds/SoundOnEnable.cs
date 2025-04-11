using Honeylab.Sounds.Data;
using UnityEngine;
using Zenject;


namespace Honeylab.Sounds
{
    public class SoundOnEnable : MonoBehaviour
    {
        [SerializeField] private SoundId _enableSound;
        [SerializeField] private SoundId _disableSound;


        private SoundService _soundService;


        [Inject]
        public void Construct(SoundService soundService)
        {
            _soundService = soundService;
        }


        public void OnEnable()
        {
            if (_enableSound != null)
            {
                _soundService.RequestSoundPlay(_enableSound);
            }
        }


        public void OnDisable()
        {
            if (_disableSound != null)
            {
                _soundService.RequestSoundPlay(_disableSound);
            }
        }
    }
}
