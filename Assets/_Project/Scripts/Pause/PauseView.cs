using AE.Core;
using AE.Core.Commands;
using AE.Core.Types;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace AE.Pause
{
    public class PauseView : MonoBehaviour
    {
        private const float AnimationDuration = 1f;
        private const Ease AnimationEase = Ease.InSine;

        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;

        private CommandBus _commandBus;
        private Sequence _currentAnimation;

        [Inject]
        private void Construct(CommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        private void Awake()
        {
            canvasGroup.alpha = 0;
            transform.localScale = Vector3.zero;

            resumeButton.onClick.AddListener(Resume);
            exitButton.onClick.AddListener(ExitGame);

            optionsButton.interactable = false;
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveListener(Resume);
            exitButton.onClick.RemoveListener(ExitGame);
        }

        public async UniTask ShowAsync()
        {
            if (_currentAnimation != null && _currentAnimation.IsPlaying())
                return;

            gameObject.SetActive(true);

            await UniTask.Yield();

            transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Join(transform.DOScale(1f, AnimationDuration).SetEase(AnimationEase));
            _currentAnimation.Join(canvasGroup.DOFade(1f, AnimationDuration).SetEase(AnimationEase));

            await _currentAnimation.AsyncWaitForCompletion();
        }

        public async UniTask HideAsync()
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (_currentAnimation != null && _currentAnimation.IsPlaying())
                return;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Join(transform.DOScale(0f, AnimationDuration).SetEase(AnimationEase));
            _currentAnimation.Join(canvasGroup.DOFade(0f, AnimationDuration).SetEase(AnimationEase));

            await _currentAnimation.AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        private void Resume()
        {
            _commandBus.Execute(new ChangeGameStateCommand(GameMode.Gameplay));
        }

        private static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}