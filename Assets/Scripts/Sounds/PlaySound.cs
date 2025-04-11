using Honeylab.Sounds.Data;
using UnityEngine;
using Zenject;


namespace Honeylab.Sounds
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] private SoundId _id;

        private SoundService _soundService;


        [Inject]
        public void Construct(SoundService soundService)
        {
            _soundService = soundService;
        }


        public void Play()
        {
            _soundService.RequestSoundPlay(_id);
        }
    }
}
