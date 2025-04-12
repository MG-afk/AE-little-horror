using System;
using System.Collections.Generic;
using AE.Core.Event;
using AE.Core.GlobalGameState;
using AE.Game.Core.Input;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AE.Core.Input
{
    [UsedImplicitly]
    public sealed class InputSystem : IDisposable
    {
        public event Action<Vector2> Moved;
        public event Action Sprinted;
        public event Action Interacted;
        public event Action Exited;
        public event Action<Vector2> Rotated;
        public event Action<float> Zoomed;
        public event Action Accepted;
        public event Action Canceled;

        private readonly InputActions _inputActions;
        private InputActionMap _currentActionMap;
        private readonly EventManager _eventManager;

        private readonly Dictionary<GameMode, InputActionMap> _actionMaps = new();

        //TODO: bind it better currently sprinted
        public bool HoldSecondKey { get; private set; }

        public InputSystem(EventManager eventManager)
        {
            _eventManager = eventManager;
            _inputActions = new InputActions();

            _inputActions.Player.SetCallbacks(new PlayerInputCallbacks(this));
            _inputActions.Inspect.SetCallbacks(new InspectInputCallbacks(this));

            _actionMaps[GameMode.Pause] = _inputActions.Inspect; //TODO: implement the UI/Pause inputs
            _actionMaps[GameMode.Inspect] = _inputActions.Inspect;
            _actionMaps[GameMode.Gameplay] = _inputActions.Player;

            _eventManager.Subscribe<GameStateEnterEvent>(SwitchToGameMode);
        }

        public void Dispose()
        {
            _eventManager.Unsubscribe<GameStateEnterEvent>(SwitchToGameMode);

            _inputActions?.Dispose();
        }

        private void SwitchToGameMode(in GameStateEnterEvent gameStateEnterEvent)
        {
            var gameMode = gameStateEnterEvent.GameMode;
            _currentActionMap?.Disable();

            if (!_actionMaps.TryGetValue(gameMode, out var actionMap))
            {
                Debug.LogWarning($"No input action map found for game mode: {gameMode}");
                return;
            }

            actionMap.Enable();
            _currentActionMap = actionMap;
        }


        private sealed class PlayerInputCallbacks : InputActions.IPlayerActions
        {
            private readonly InputSystem _inputSystem;

            public PlayerInputCallbacks(InputSystem inputSystem)
            {
                _inputSystem = inputSystem;
            }

            public void OnMove(InputAction.CallbackContext context)
            {
                _inputSystem.Moved?.Invoke(context.ReadValue<Vector2>());
            }

            public void OnLook(InputAction.CallbackContext context)
            {
            }

            public void OnInteract(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Interacted?.Invoke();
                }
            }

            public void OnExit(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Exited?.Invoke();
                }
            }

            public void OnSprint(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Sprinted?.Invoke();
                }

                _inputSystem.HoldSecondKey = context.performed;
            }
        }

        private sealed class InspectInputCallbacks : InputActions.IInspectActions
        {
            private readonly InputSystem _inputSystem;

            public InspectInputCallbacks(InputSystem inputSystem)
            {
                _inputSystem = inputSystem;
            }

            public void OnRotate(InputAction.CallbackContext context)
            {
                _inputSystem.Rotated?.Invoke(context.ReadValue<Vector2>());
            }

            public void OnZoom(InputAction.CallbackContext context)
            {
                _inputSystem.Zoomed?.Invoke(context.ReadValue<float>());
            }

            public void OnAccept(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Accepted?.Invoke();
                }
            }

            public void OnCancel(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Canceled?.Invoke();
                }
            }
        }
    }
}