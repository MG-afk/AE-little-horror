using AE.Core;
using AE.Core.Commands;
using AE.Core.GlobalGameState;
using AE.Riddle;
using UnityEngine;
using VContainer;

namespace AE.Interactions.Trigger
{
    public class Ghost : BaseTrigger
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

            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnDestroy()
        {
            _blackboard.NewValueSet -= OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (key == RiddleConstant.EndKey && value == RiddleConstant.EndValue)
            {
                _commandBus.Execute(new ChangeGameStateCommand(GameMode.GameOver));
            }
        }


        protected override void OnTriggerEnter(Collider other)
        {
        }
    }
}