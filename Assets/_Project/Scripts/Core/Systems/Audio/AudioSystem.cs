using System.Collections.Generic;
using System.Linq;
using AE.Core.Event;
using AE.Core.GlobalGameState;
using Cysharp.Threading.Tasks;
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

        private readonly Dictionary<GameMode, AudioSource> _gameStateAudioSources = new();
        private readonly List<AudioSource> _audioPool = new();
        private readonly Dictionary<SoundType, AudioClip> _soundCache = new();
        private readonly List<UniTaskCompletionSource> _activeAudioTasks = new();

        private EventManager _eventManager;

        [Inject]
        private void Construct(EventManager eventManager)
        {
            _eventManager = eventManager;
            InitializeGameStateAudioSources();
            _eventManager.Subscribe<GameStateEnterEvent>(OnGameStateChanged);
            InitializeAudioPool();
            CacheSounds();
        }

        private void OnDestroy()
        {
            if (_eventManager != null)
            {
                _eventManager.Unsubscribe<GameStateEnterEvent>(OnGameStateChanged);
            }

            foreach (var task in _activeAudioTasks)
            {
                task.TrySetCanceled();
            }

            _activeAudioTasks.Clear();
        }

        private void InitializeGameStateAudioSources()
        {
            _gameStateAudioSources[GameMode.Pause] = pauseAudio;
            _gameStateAudioSources[GameMode.Gameplay] = gameplayAudio;
            _gameStateAudioSources[GameMode.Inspect] = inspectionAudio;
        }

        private void OnGameStateChanged(in GameStateEnterEvent evt)
        {
            foreach (var audioSource in _gameStateAudioSources.Values)
            {
                audioSource.Stop();
            }

            if (_gameStateAudioSources.TryGetValue(evt.GameMode, out var selectedAudio))
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
            {
                Debug.LogWarning("SoundsDatabase is not assigned to AudioSystem");
                return;
            }

            foreach (var soundType in System.Enum.GetValues(typeof(SoundType)).Cast<SoundType>())
            {
                var clip = soundsDatabase.GetSound(soundType);
                if (clip != null)
                {
                    _soundCache[soundType] = clip;
                }
            }
        }

        public void PlaySound(SoundType soundType, float volumeMultiplier = 1f)
        {
            if (soundType == SoundType.None)
                return;

            var clip = GetSoundClip(soundType);

            if (clip == null)
                return;

            var source = GetAvailableAudioSource();

            if (source == null)
                return;

            source.transform.position = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            source.clip = clip;
            source.volume = volumeMultiplier;
            source.spatialBlend = 0f;
            source.gameObject.SetActive(true);
            source.Play();

            DisableAfterPlayAsync(source).Forget();
        }

        public AudioSource PlaySound(SoundType soundType, Vector3 position, float volumeMultiplier = 1f)
        {
            if (soundType == SoundType.None)
                return null;

            AudioClip clip = GetSoundClip(soundType);
            if (clip == null)
                return null;

            AudioSource source = GetAvailableAudioSource();
            if (source == null)
                return null;

            ConfigurePositionalAudioSource(source, position, clip, volumeMultiplier);

            DisableAfterPlayAsync(source).Forget();
            return source;
        }

        public void PlayHorrorSound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
                return;

            AudioSource source = GetAvailableAudioSource();
            if (source == null)
                return;

            ConfigurePositionalAudioSource(source, position, clip, volume);

            DisableAfterPlayAsync(source).Forget();
        }

        private AudioClip GetSoundClip(SoundType soundType)
        {
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
            }

            return clip;
        }

        private void ConfigurePositionalAudioSource(AudioSource source, Vector3 position, AudioClip clip, float volume)
        {
            source.transform.position = position;
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.minDistance = 1f;
            source.maxDistance = 15f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.gameObject.SetActive(true);
            source.Play();
        }

        private AudioSource GetAvailableAudioSource()
        {
            var source = _audioPool.FirstOrDefault(source => !source.isPlaying);
            if (source == null)
            {
                Debug.LogWarning("No available audio sources in pool. Consider increasing pool size.");
            }

            return source;
        }

        private async UniTaskVoid DisableAfterPlayAsync(AudioSource source)
        {
            if (source == null || source.clip == null)
                return;

            var completionSource = new UniTaskCompletionSource();
            _activeAudioTasks.Add(completionSource);

            try
            {
                await UniTask.Delay((int)(source.clip.length * 1000),
                    cancellationToken: this.GetCancellationTokenOnDestroy());

                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
            }
            catch (System.OperationCanceledException)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
            }
            finally
            {
                _activeAudioTasks.Remove(completionSource);
                completionSource.TrySetResult();
            }
        }
    }
}