using DG.Tweening;
using UnityEngine;

namespace AE.Interactions.Manipulable
{
    public class ManipulationHintUI : MonoBehaviour
    {
        [SerializeField] private GameObject holdingPanel;
        [SerializeField] private GameObject placementPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        private Sequence _fadeTween;

        private void Awake()
        {
            Hide();
        }

        public void ShowHoldingHints()
        {
            holdingPanel.SetActive(true);
            placementPanel.SetActive(false);
            FadeIn();
        }

        public void ShowPlacementHints()
        {
            holdingPanel.SetActive(false);
            placementPanel.SetActive(true);
            FadeIn();
        }

        public void Hide()
        {
            _fadeTween?.Kill();

            _fadeTween = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0, 1f)
                    .OnComplete(() =>
                    {
                        holdingPanel.SetActive(false);
                        placementPanel.SetActive(false);
                    }));
        }

        private void FadeIn()
        {
            _fadeTween?.Kill();

            canvasGroup.alpha = 0;
            _fadeTween = DOTween.Sequence().Append(canvasGroup.DOFade(1, 0.5f));
        }
    }
}