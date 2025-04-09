using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public class GameTimeSystem
    {
        private const float DefaultTimeScale = 1.0f;

        public void PauseTime()
        {
            Time.timeScale = 0f;
            Debug.Log("Game time paused");
        }

        public void ResumeTime()
        {
            Time.timeScale = DefaultTimeScale;
            Debug.Log("Game time resumed");
        }

        public void SlowDownTime(float scale)
        {
            Time.timeScale = scale;
            Debug.Log($"Game time slowed down to {scale}");
        }
    }
}