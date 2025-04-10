using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.GlobalGameState;
using AE.Core.Types;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource gameplayAudio;
        [SerializeField] private AudioSource inspectionAudio;
        [SerializeField] private AudioSource pauseAudio;

        private readonly Dictionary<GameMode, AudioSource> _audioSystems = new();

        private EventManager _eventManager;

        [Inject]
        private void Construct(EventManager eventManager)
        {
            _eventManager = eventManager;

            _audioSystems[GameMode.Pause] = pauseAudio;
            _audioSystems[GameMode.Gameplay] = gameplayAudio;
            _audioSystems[GameMode.Inspect] = inspectionAudio;

            _eventManager.Subscribe<GameStateEnterEvent>(SwitchAudio);
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

            _audioSystems[evt.GameMode].Play();
        }
    }
}