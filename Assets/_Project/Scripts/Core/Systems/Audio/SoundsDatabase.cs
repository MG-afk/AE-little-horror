using System.Collections.Generic;
using UnityEngine;

namespace AE.Core.Systems.Audio
{
    [CreateAssetMenu(fileName = "SoundsDatabase", menuName = "AE/Audio/Sounds Database")]
    public class SoundsDatabase : ScriptableObject
    {
        [SerializeField] private List<SoundTypeMapping> sounds = new();

        private Dictionary<SoundType, AudioClip> _soundDictionary;
        private Dictionary<SoundType, float> _volumeDictionary;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            _soundDictionary = new Dictionary<SoundType, AudioClip>();
            _volumeDictionary = new Dictionary<SoundType, float>();

            foreach (var sound in sounds)
            {
                if (sound.clip == null)
                    continue;

                _soundDictionary[sound.soundType] = sound.clip;
                _volumeDictionary[sound.soundType] = sound.defaultVolume;
            }
        }

        public AudioClip GetSound(SoundType soundType)
        {
            if (_soundDictionary == null)
            {
                InitializeDictionary();
            }

            return _soundDictionary.GetValueOrDefault(soundType);
        }

        public float GetDefaultVolume(SoundType soundType)
        {
            if (_volumeDictionary == null)
            {
                InitializeDictionary();
            }

            return _volumeDictionary.GetValueOrDefault(soundType, 1f);
        }

        public void AddOrUpdateSound(SoundType soundType, AudioClip clip, float defaultVolume = 1f)
        {
            if (_soundDictionary == null)
            {
                InitializeDictionary();
            }

            _soundDictionary[soundType] = clip;
            _volumeDictionary[soundType] = defaultVolume;

            var updated = false;
            for (var i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].soundType != soundType)
                    continue;

                sounds[i].clip = clip;
                sounds[i].defaultVolume = defaultVolume;
                updated = true;
                break;
            }

            if (!updated)
            {
                sounds.Add(new SoundTypeMapping
                {
                    soundType = soundType,
                    clip = clip,
                    defaultVolume = defaultVolume
                });
            }
        }
    }
}