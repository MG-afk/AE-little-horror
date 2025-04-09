using AE.Core;
using DG.Tweening;
using JetBrains.Annotations;
using VContainer.Unity;

namespace AE
{
    [UsedImplicitly]
    public sealed class Bootstrapper : IStartable
    {
        private readonly Utilities _utilities;

        public Bootstrapper(Utilities utilities)
        {
            _utilities = utilities;
        }

        public void Start()
        {
            var canvasGroup = _utilities.FadeCanvasGroup;

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, 1f).SetEase(Ease.Linear)
                .OnComplete(() => { canvasGroup.gameObject.SetActive(false); });
        }
    }
}