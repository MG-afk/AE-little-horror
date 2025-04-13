using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AE.SimplifyDialogue
{
    [RequireComponent(typeof(Canvas))]
    public class SimplifyDialogueView : MonoBehaviour
    {
        private const float FadeInDuration = .25f;
        private const Ease FadeInEase = Ease.OutQuad;
        private const float FadeOutDuration = 1.2f;
        private const Ease FadeOutEase = Ease.InQuad;

        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _isAnimating;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            dialogueText.text = "";
        }

        public async UniTask Show(string text, float duration = 1f)
        {
            if (_isAnimating)
                return;

            _isAnimating = true;

            dialogueText.text = text;

            canvasGroup.alpha = 0f;
            dialogueText.alpha = 0f;

            await canvasGroup.DOFade(1f, FadeInDuration).SetEase(FadeInEase).AsyncWaitForCompletion();
            await dialogueText.DOFade(1f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            await Hide();
        }

        private async UniTask Hide()
        {
            await dialogueText.DOFade(0f, 0.5f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            await canvasGroup.DOFade(0f, FadeOutDuration).SetEase(FadeOutEase).AsyncWaitForCompletion();

            _isAnimating = false;
        }
    }
}