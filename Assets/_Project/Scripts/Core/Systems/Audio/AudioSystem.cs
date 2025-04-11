using System.Collections.Generic;
using System.Linq;
using AE.Core.Event;
using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace AE.Core.Systems.Audio
{
    [UsedImplicitly]
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField] private SoundsDatabase soundsDatabase;

        [SerializeField] private AudioSource gameplayAudio;
        [SerializeField] private AudioSource inspectionAudio;
        [SerializeField] private AudioSource pauseAudio;

        [SerializeField] private int pooledAudioSourceCount = 10;
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private Transform audioSourcePoolParent;

        private readonly Dictionary<GameMode, AudioSource> _audioSystems = new();
        private readonly List<AudioSource> _audioPool = new();
        private readonly Dictionary<SoundType, AudioClip> _soundCache = new();

        private EventManager _eventManager;

        [Inject]
        private void Construct(EventManager eventManager)
        {
            _eventManager = eventManager;

            _audioSystems[GameMode.Pause] = pauseAudio;
            _audioSystems[GameMode.Gameplay] = gameplayAudio;
            _audioSystems[GameMode.Inspect] = inspectionAudio;

            _eventManager.Subscribe<GameStateEnterEvent>(SwitchAudio);

            InitializeAudioPool();
            CacheSounds();
        }

        private void OnDestroy()
        {
            _eventManager.Unsubscribe<GameStateEnterEvent>(SwitchAudio);
        }

        private void SwitchAudio(in GameStateEnterEvent evt)
        {
            foreach (var audioSystem in _audioSystems)
            {
                audioSystem.Value.Stop();
            }

            if (_audioSystems.TryGetValue(evt.GameMode, out var selectedAudio))
            {
                selectedAudio.Play();
            }
        }

        private void InitializeAudioPool()
        {
            for (var i = 0; i < pooledAudioSourceCount; i++)
            {
                var source = Instantiate(audioSourcePrefab, audioSourcePoolParent);
                source.gameObject.SetActive(false);
                _audioPool.Add(source);
            }
        }

        private void CacheSounds()
        {
            if (soundsDatabase == null)
                return;

            foreach (var soundType in System.Enum.GetValues(typeof(SoundType)))
            {
                var clip = soundsDatabase.GetSound((SoundType)soundType);
                if (clip != null)
                {
                    _soundCache[(SoundType)soundType] = clip;
                }
            }
        }

        public void PlaySound(SoundType soundType, float volumeMultiplier = 1f)
        {
            if (soundType == SoundType.None)
                return;

            if (!_soundCache.TryGetValue(soundType, out var clip))
            {
                clip = soundsDatabase?.GetSound(soundType);

                if (clip != null)
                {
                    _soundCache[soundType] = clip;
                }
            }

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found for type: {soundType}");
                return;
            }

            var source = GetAvailableAudioSource();

            if (source == null)
                return;

            PlaySoundOnSource(source, clip, volumeMultiplier);
        }

        private void PlaySoundOnSource(AudioSource source, AudioClip clip, float volumeMultiplier)
        {
            source.transform.position = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            source.clip = clip;
            source.volume = volumeMultiplier;
            source.spatialBlend = 0f; // 2D sound for UI
            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(DisableAfterPlay(source));
        }

        public void PlayHorrorSound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            var source = GetAvailableAudioSource();

            if (source == null || clip == null)
                return;

            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.minDistance = 1f;
            source.maxDistance = 15f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(DisableAfterPlay(source));
        }

        public AudioSource PlaySound(SoundType soundType, Vector3 position, float volumeMultiplier = 1f)
        {
            if (soundType == SoundType.None)
                return null;

            if (!_soundCache.TryGetValue(soundType, out AudioClip clip))
            {
                clip = soundsDatabase?.GetSound(soundType);

                if (clip != null)
                {
                    _soundCache[soundType] = clip;
                }
            }

            if (clip == null)
            {
                Debug.LogWarning($"Sound not found for type: {soundType}");
                return null;
            }

            var source = GetAvailableAudioSource();
            if (source == null)
                return null;

            source.transform.position = position;
            source.clip = clip;
            source.volume = volumeMultiplier;
            source.spatialBlend = 1f;
            source.minDistance = 1f;
            source.maxDistance = 15f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(DisableAfterPlay(source));

            return source;
        }

        private AudioSource GetAvailableAudioSource()
        {
            return _audioPool.FirstOrDefault(source => !source.isPlaying);
        }

        private static System.Collections.IEnumerator DisableAfterPlay(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
        }
    }
}