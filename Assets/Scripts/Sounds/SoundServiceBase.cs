using Honeylab.Sounds.Data;
using Honeylab.Utils.Prefs;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using static Honeylab.Sounds.Data.AudioSourceType;


namespace Honeylab.Sounds
{
    [Serializable]
    public class SoundServiceParams
    {
        public Transform CommonAudioSourceRoot;
        public AudioMixerGroup DefaultAudioMixerGroup;
        public bool NoSoundException;
    }


    public abstract class SoundServiceBase : IDisposable
    {
        public const string MusicEnabled = nameof(MusicEnabled);
        public const string SfxEnabled = nameof(SfxEnabled);

        private readonly SoundsData _data;
        private readonly SoundServiceParams _params;
        private readonly IPrefsService _prefsService;

        private readonly ReactiveProperty<bool> _musicEnabledProp = new ReactiveProperty<bool>(true);
        private readonly ReactiveProperty<bool> _sfxEnabledProp = new ReactiveProperty<bool>(true);
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private readonly Dictionary<SoundId, AudioSource> _sources = new Dictionary<SoundId, AudioSource>();
        private AudioSource _music;
        private AudioSource _sfx;

        public ReactiveProperty<bool> MusicEnabledProp => _musicEnabledProp;
        public ReactiveProperty<bool> SfxEnabledProp => _sfxEnabledProp;


        protected SoundServiceBase(SoundsData data, SoundServiceParams p, IPrefsService prefsService)
        {
            _data = data;
            _params = p;
            _prefsService = prefsService;
        }


        public void Init()
        {
            _music = CreateAudioSource();
            _sfx = CreateAudioSource();

            var sounds = _data.Sounds;
            sounds.ForEach(it =>
            {
                switch (it.CommonSourceType)
                {
                    case None:
                        AudioSource source = CreateAudioSource();
                        _sources.Add(it.Id, source);
                        break;

                    case Music:
                        _sources.Add(it.Id, _music);
                        break;

                    case OneShot:
                        _sources.Add(it.Id, _sfx);
                        break;

                    default:
                        throw new ArgumentException();
                }
            });

            if (_prefsService.HasKey(MusicEnabled))
            {
                MusicEnabledProp.Value = _prefsService.GetBool(MusicEnabled);
            }

            if (_prefsService.HasKey(SfxEnabled))
            {
                SfxEnabledProp.Value = _prefsService.GetBool(SfxEnabled);
            }
        }


        public void Run()
        {
            IDisposable isMusicEnabled = MusicEnabledProp.Subscribe(isEnabled =>
            {
                _music.mute = !isEnabled;
                _prefsService.SetBool(MusicEnabled, isEnabled);
            });
            _disposable.Add(isMusicEnabled);

            IDisposable isSfxEnabled = SfxEnabledProp.Subscribe(isEnabled =>
            {
                _sfx.mute = !isEnabled;
                _prefsService.SetBool(SfxEnabled, isEnabled);
            });
            _disposable.Add(isSfxEnabled);
        }


        public void Dispose()
        {
            _disposable?.Dispose();
        }


        public bool IsSoundPlaying(SoundId soundId)
        {
            SoundBinding sound = _data.GetSound(soundId);
            if (sound == null)
            {
                return false;
            }

            AudioSource source = _sources[sound.Id];
            return source != null && source.isPlaying;
        }


        public void RequestSoundPlay(SoundId soundId)
        {
            SoundBinding sound = _data.GetSound(soundId);
            if (sound == null)
            {
                if (_params.NoSoundException)
                {
                    throw new Exception($"No sound: {soundId.name}");
                }

                Debug.LogWarning($"No sound");
                return;
            }

            AudioSource source = _sources[sound.Id];
            if (source == null)
            {
                throw new Exception($"No audio source for sound: {soundId.name}");
            }

            source.loop = sound.Loop;
            source.volume = sound.Volume;
            AudioClip clip = sound.GetClip();
            if (clip == null)
            {
                Debug.LogWarning($"No audio clip found for {soundId.name}");
            }

            switch (sound.CommonSourceType)
            {
                case None:
                case Music:
                    source.clip = clip;
                    source.Play();
                    break;

                case OneShot:
                    source.PlayOneShot(clip, source.volume);
                    break;

                default:
                    throw new ArgumentException();
            }
        }


        public void RequestSoundStop(SoundId soundId)
        {
            SoundBinding sound = _data.GetSound(soundId);
            if (sound == null)
            {
                if (_params.NoSoundException)
                {
                    throw new Exception($"No sound: {soundId.name}");
                }

                Debug.LogWarning($"No sound");
                return;
            }

            AudioSource source = _sources[sound.Id];
            if (sound == null)
            {
                throw new Exception($"No audio source for sound: {soundId.name}");
            }

            switch (sound.CommonSourceType)
            {
                case None:
                case Music:
                    source.Stop();
                    break;

                default:
                    throw new ArgumentException();
            }
        }


        private AudioSource CreateAudioSource()
        {
            AudioSource audioSource = _params.CommonAudioSourceRoot.gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _params.DefaultAudioMixerGroup;
            return audioSource;
        }
    }
}
