using System;
using System.Collections.Generic;
using AE.Core.Types;
using AE.Game.Core.Input;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AE.Core.Systems
{
    [UsedImplicitly]
    public sealed class InputSystem : IDisposable
    {
        public event Action<Vector2> Moved;
        public event Action Interacted;
        public event Action<Vector2> Rotated;
        public event Action<float> Zoomed;
        public event Action Accepted;
        public event Action Canceled;
        public event Action<GameMode> GameModeChanged;

        private readonly InputActions _inputActions;
        private InputActionMap _currentActionMap;
        private GameMode _currentGameMode;

        private readonly Dictionary<GameMode, InputActionMap> _actionMaps = new();

        public InputSystem()
        {
            _inputActions = new InputActions();

            _inputActions.Player.SetCallbacks(new PlayerInputCallbacks(this));
            _inputActions.Inspect.SetCallbacks(new InspectInputCallbacks(this));

            _actionMaps[GameMode.Pause] = null; //TODO: implement the UI/Pause inputs
            _actionMaps[GameMode.Inspect] = _inputActions.Inspect;
            _actionMaps[GameMode.Gameplay] = _inputActions.Player;

            SwitchToGameMode(GameMode.Gameplay);
        }

        public void Dispose()
        {
            _inputActions?.Dispose();
        }

        public void SwitchToGameMode(GameMode gameMode)
        {
            _currentActionMap?.Disable();

            if (!_actionMaps.TryGetValue(gameMode, out var actionMap))
            {
                Debug.LogWarning($"No input action map found for game mode: {gameMode}");
                return;
            }

            actionMap.Enable();
            _currentActionMap = actionMap;
            _currentGameMode = gameMode;
            GameModeChanged?.Invoke(gameMode);
            Debug.Log($"Switched input to {gameMode} mode");
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

            public void OnInteract(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    _inputSystem.Interacted?.Invoke();
                }
            }

            public void OnPause(InputAction.CallbackContext context)
            {
                if (context.performed)
                {
                    //TODO: pause input
                }
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