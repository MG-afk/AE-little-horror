using System.Collections.Generic;
using UnityEngine;

namespace AE.Core.Systems.Audio
{
    [CreateAssetMenu(fileName = "SoundsDatabase", menuName = "AE/Audio/Sounds Database")]
    public class SoundsDatabase : ScriptableObject
    {
        [SerializeField] private List<SoundTypeMapping> sounds = new();

        private Dictionary<SoundType, AudioClip> _soundDictionary;

        private void OnEnable()
        {
            InitializeDictionary();
        }

        private void InitializeDictionary()
        {
            _soundDictionary = new Dictionary<SoundType, AudioClip>();

            foreach (var sound in sounds)
            {
                if (sound.clip == null)
                    continue;

                _soundDictionary[sound.soundType] = sound.clip;
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
    }
}