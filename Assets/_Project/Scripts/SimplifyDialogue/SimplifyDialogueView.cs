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
        [SerializeField] private List<AudioClip> horrorStingers;

        private bool isAnimating = false;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            dialogueText.text = "";

            if (typingSoundSource == null)
            {
                typingSoundSource = gameObject.AddComponent<AudioSource>();
                typingSoundSource.volume = 0.5f;
                typingSoundSource.pitch = 0.9f;
            }
        }

        public async UniTask Show(string text)
        {
            if (isAnimating)
            {
                DOTween.Kill(dialogueText);
            }

            isAnimating = true;

            dialogueText.text = text;
            ApplyHorrorStyling();

            canvas.enabled = true;
            canvasGroup.alpha = 0f;
            dialogueText.alpha = 0f;

            await canvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeInEase).AsyncWaitForCompletion();
            await dialogueText.DOFade(1f, 1.2f).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

            PlayRandomStinger();

            isAnimating = false;

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

        private void PlayRandomStinger()
        {
            if (horrorStingers != null && horrorStingers.Count > 0 && typingSoundSource != null)
            {
                if (Random.value < 0.5f)
                {
                    var stinger = horrorStingers[Random.Range(0, horrorStingers.Count)];
                    typingSoundSource.PlayOneShot(stinger, Random.Range(0.3f, 0.6f));
                }
            }
        }
    }
}