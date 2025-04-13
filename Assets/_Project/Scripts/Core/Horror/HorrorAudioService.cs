using AE.Core.Systems.Audio;
using AE.Core.Utility;
using JetBrains.Annotations;
using UnityEngine;
using VContainer.Unity;

namespace AE.Core.Horror
{
    [UsedImplicitly]
    public class HorrorAudioService : ITickable
    {
        private readonly AudioSystem _audioSystem;
        private readonly Transform _playerTransform;

        private const float MinTimeBetweenSounds = 8f;
        private const float MaxTimeBetweenSounds = 25f;
        private const float MinDistance = 5f;
        private const float MaxDistance = 15f;
        private const float MinPitch = 0.8f;
        private const float MaxPitch = 1.2f;
        private const float MinVolume = 0.7f;
        private const float MaxVolume = 1.0f;

        private float _nextSoundTime;

        public HorrorAudioService(AudioSystem audioSystem, Utilities utilities)
        {
            _audioSystem = audioSystem;
            _playerTransform = utilities.PlayerGameObject.transform;

            ScheduleNextSound();
        }

        public void Tick()
        {
            if (!(Time.time >= _nextSoundTime) || _audioSystem.HorrorSounds.Length <= 0)
                return;

            PlayRandomHorrorSound();
            ScheduleNextSound();
        }

        private void PlayRandomHorrorSound()
        {
            var sound = _audioSystem.HorrorSounds[Random.Range(0, _audioSystem.HorrorSounds.Length)];

            var randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
            randomDirection.Normalize();

            var distance = Random.Range(MinDistance, MaxDistance);
            var soundPosition = _playerTransform.position + (randomDirection * distance);

            var volume = Random.Range(MinVolume, MaxVolume);
            var pitch = Random.Range(MinPitch, MaxPitch);

            _audioSystem.PlayHorrorSound(sound, soundPosition, volume, pitch);
        }

        private void ScheduleNextSound()
        {
            _nextSoundTime = Time.time + Random.Range(MinTimeBetweenSounds, MaxTimeBetweenSounds);
        }
    }
}