using System;
using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public class GameTimeSystem : IDisposable
    {
        private const float DefaultTimeScale = 1.0f;
        private const float SlowTimeScale = 0.1f;

        private readonly EventManager _eventManager;
        private readonly Dictionary<GameMode, float> _gameTime = new();

        public GameTimeSystem(EventManager eventManager)
        {
            _eventManager = eventManager;

            _gameTime[GameMode.Pause] = 0f;
            _gameTime[GameMode.Gameplay] = DefaultTimeScale;
            _gameTime[GameMode.Inspect] = SlowTimeScale;

            _eventManager.Subscribe<GameStateEnterEvent>(ChangeTime);
        }

        private void ChangeTime(in GameStateEnterEvent evt)
        {
            Time.timeScale = _gameTime[evt.GameMode];
        }

        public void Dispose()
        {
            _eventManager.Unsubscribe<GameStateEnterEvent>(ChangeTime);
        }
    }
}