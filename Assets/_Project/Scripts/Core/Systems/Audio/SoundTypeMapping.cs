using System;
using UnityEngine;

namespace AE.Core.Systems.Audio
{
    [Serializable]
    public class SoundTypeMapping
    {
        public SoundType soundType;
        public AudioClip clip;
    }
}