using DG.Tweening;

namespace AE.Core.Utility
{
    public static class UtilitiesExtension
    {
        public static void FadeOut(this Utilities utility, float duration)
        {
            var canvasGroup = utility.FadeCanvasGroup;

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)
                .OnComplete(() => { canvasGroup.gameObject.SetActive(false); });
        }

        public static void FadeIn(this Utilities utility, float duration)
        {
            var canvasGroup = utility.FadeCanvasGroup;

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, duration).SetEase(Ease.Linear);
        }
    }
}