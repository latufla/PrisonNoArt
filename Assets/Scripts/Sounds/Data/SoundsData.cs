using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Honeylab.Sounds.Data
{
    public enum AudioSourceType
    {
        None,
        Music,
        OneShot
    }


    [Serializable]
    public class SoundBinding
    {
        [Tooltip("Editor only")]
        [SerializeField] private string _name;

        [SerializeField] private SoundId _id;
        [SerializeField] private AudioSourceType _commonSourceType;

        [SerializeField] private bool _loop;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float _volume = 1.0f;

        [SerializeField] private List<AudioClip> _clips;

        public AudioSourceType CommonSourceType => _commonSourceType;
        public bool Loop => _loop;
        public float Volume => _volume;
        public SoundId Id => _id;


        public string Name
        {
            get => _name;
            set => _name = value;
        }


        public AudioClip GetClip() => _clips.Count != 0 ? _clips[Random.Range(0, _clips.Count)] : null;
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + "Sounds",
        menuName = DataUtil.MenuNamePrefix + "Sounds")]
    public class SoundsData : ScriptableObject
    {
        [SerializeField] private SoundId _mainMusicSoundId;
        [SerializeField] private List<SoundBinding> _sounds;

        public SoundId MainMusicSoundId => _mainMusicSoundId;
        public List<SoundBinding> Sounds => _sounds;


        private void OnValidate()
        {
            Sounds.ForEach(it => it.Name = it.Id.name.Replace("Data_Sounds_Id_", string.Empty));
        }


        public SoundBinding GetSound(SoundId id) => _sounds.FirstOrDefault(it => it.Id.Equals(id));
    }
}
