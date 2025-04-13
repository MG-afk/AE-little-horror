using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Ghost : RiddleItem
    {
        [SerializeField] private AudioSource audioSource;

        private RiddleBlackboard _blackboard;
        private CommandBus _commandBus;

        public AudioSource AudioSource => audioSource;

        [Inject]
        private void Construct(RiddleBlackboard blackboard, CommandBus commandBus)
        {
            _blackboard = blackboard;
            _commandBus = commandBus;
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void StartGameOver()
        {
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.GameOver));
        }
    }
}