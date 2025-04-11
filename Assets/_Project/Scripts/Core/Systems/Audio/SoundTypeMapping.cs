using System;
using UnityEngine;

namespace AE.Core.Systems.Audio
{
    [Serializable]
    public class SoundTypeMapping
    {
        public SoundType soundType;
        public AudioClip clip;
        [Range(0f, 1f)] public float defaultVolume = 1f;
    }
}