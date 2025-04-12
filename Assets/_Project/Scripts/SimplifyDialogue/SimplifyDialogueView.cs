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

        [Header("References")] [SerializeField]
        private TextMeshProUGUI dialogueText;

        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image backgroundPanel;

        [SerializeField] private float fadeOutDuration = 1.0f;
        [SerializeField] private Ease fadeInEase = Ease.OutQuad;
        [SerializeField] private Ease fadeOutEase = Ease.InQuad;

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

            canvas.enabled = true;
            canvasGroup.alpha = 0f;
            dialogueText.alpha = 0f;

            await canvasGroup.DOFade(1f, FadeInDuration).SetEase(fadeInEase).AsyncWaitForCompletion();
            await dialogueText.DOFade(1f, duration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            await Hide();
        }

        private async UniTask Hide()
        {
            await dialogueText.DOFade(0f, 0.5f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            await canvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase).AsyncWaitForCompletion();

            canvas.enabled = false;
            _isAnimating = false;
        }
    }
}