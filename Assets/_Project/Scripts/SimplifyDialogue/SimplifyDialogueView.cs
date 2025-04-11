using System.Collections.Generic;
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
        [Header("References")] [SerializeField]
        private TextMeshProUGUI dialogueText;

        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image backgroundPanel;

        [Header("Animation Settings")] [SerializeField]
        private float fadeInDuration = 1.0f;

        [SerializeField] private float fadeOutDuration = 1.0f;
        [SerializeField] private Ease fadeInEase = Ease.OutQuad;
        [SerializeField] private Ease fadeOutEase = Ease.InQuad;

        [Header("Audio")] [SerializeField] private AudioSource typingSoundSource;
        [SerializeField] private AudioClip horrorStinger;

        private bool _isAnimating;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            dialogueText.text = "";

            if (typingSoundSource != null)
                return;

            typingSoundSource = gameObject.AddComponent<AudioSource>();
            typingSoundSource.volume = 0.5f;
            typingSoundSource.pitch = 0.9f;
        }

        public async UniTask Show(string text)
        {
            if (_isAnimating)
            {
                DOTween.Kill(dialogueText);
            }

            _isAnimating = true;

            dialogueText.text = text;
            ApplyHorrorStyling();

            canvas.enabled = true;
            canvasGroup.alpha = 0f;
            dialogueText.alpha = 0f;

            await canvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeInEase).AsyncWaitForCompletion();

            typingSoundSource.PlayOneShot(horrorStinger, Random.Range(0.3f, 0.6f));

            await dialogueText.DOFade(1f, 1.2f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            _isAnimating = false;

            await Hide();
        }

        private async UniTask Hide()
        {
            await dialogueText.DOFade(0f, 0.5f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
            await canvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeOutEase).AsyncWaitForCompletion();

            canvas.enabled = false;
        }

        private void ApplyHorrorStyling()
        {
            dialogueText.fontStyle = FontStyles.Bold;
            dialogueText.characterSpacing = Random.Range(-2f, 2f);
            dialogueText.enableVertexGradient = true;
        }
    }
}