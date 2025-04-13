using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE.Interactions.Objects
{
    public class Candlesticks : InteractableItem
    {
        [Header("Candlesticks")] [SerializeField]
        private GameObject[] fires;

        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Light[] lights;
        [SerializeField] private AnimationCurve colorTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Gradient flameGradient;
        [SerializeField] private float lightIntensity = 1.5f;
        [SerializeField] private float lightRange = 3.0f;

        private Color _originalRendererColor;

        private void Awake()
        {
            _originalRendererColor = renderers[0].material.color;
        }

        public void Activate(bool value)
        {
            SetActivateCandlesticks(value).Forget();
        }

        public void ChangeColorToBlue()
        {
            ChangeColorAsync(Color.blue, 10f).Forget();
        }

        private async UniTask SetActivateCandlesticks(bool activate)
        {
            foreach (var fire in fires)
            {
                fire.SetActive(activate);

                if (activate)
                {
                    await UniTask.Delay(1000);
                }
            }
        }

        private async UniTask ChangeColorAsync(Color targetColor, float duration)
        {
            var sequence = DOTween.Sequence();

            foreach (var par in particles)
            {
                var main = par.main;
                var initialColor = main.startColor.color;

                sequence.Join(
                    DOTween.To(
                        () => initialColor,
                        color =>
                        {
                            main.startColor = new ParticleSystem.MinMaxGradient(color);

                            var colorOverLifetime = par.colorOverLifetime;
                            colorOverLifetime.enabled = true;
                            colorOverLifetime.color = flameGradient;
                        },
                        targetColor,
                        duration
                    ).SetEase(colorTransitionCurve)
                );
            }

            foreach (var render in renderers)
            {
                sequence.Join(
                    render.material.DOColor(targetColor, duration)
                        .SetEase(colorTransitionCurve)
                        .From(_originalRendererColor)
                );
            }

            foreach (var lig in lights)
            {
                sequence.Join(
                    lig.DOColor(targetColor, duration)
                        .SetEase(colorTransitionCurve)
                );

                sequence.Join(
                    lig.DOIntensity(lightIntensity, duration)
                        .SetEase(colorTransitionCurve)
                );

                var tempLig = lig;
                sequence.Join(
                    DOTween.To(
                        () => tempLig.range,
                        range => lig.range = range,
                        lightRange,
                        duration
                    ).SetEase(colorTransitionCurve)
                );
            }

            await sequence.Play().AsyncWaitForCompletion();

            FinalizeColorChange(targetColor);
        }

        private void FinalizeColorChange(Color targetColor)
        {
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startColor = new ParticleSystem.MinMaxGradient(targetColor);

                var colorOverLifetime = particle.colorOverLifetime;
                colorOverLifetime.enabled = true;
                colorOverLifetime.color = flameGradient;
            }

            foreach (var ren in renderers)
            {
                ren.material.color = targetColor;
            }

            foreach (var lig in lights)
            {
                lig.color = targetColor;
                lig.intensity = lightIntensity;
                lig.range = lightRange;
            }
        }
    }
}