using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using AE.Riddle;
using DG.Tweening;
using VContainer;

namespace AE.Interactions.Objects
{
    public class Candlesticks : InteractableItem
    {
        [SerializeField] private GameObject[] fires;

        [SerializeField] private ParticleSystem[] particles;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Light[] lights;

        [SerializeField] private float colorTransitionTime = 1.0f;
        [SerializeField] private AnimationCurve colorTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Gradient flameGradient;

        [SerializeField] private float lightIntensity = 1.5f;
        [SerializeField] private float lightRange = 3.0f;

        private RiddleBlackboard _blackboard;

        private readonly List<Color> _originalRendererColors = new();
        private readonly List<Color> _originalLightColors = new();

        [Inject]
        private void Construct(RiddleBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        private void Awake()
        {
            foreach (var renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    _originalRendererColors.Add(renderer.material.color);
                }
            }

            foreach (var light in lights)
            {
                if (light != null)
                {
                    _originalLightColors.Add(light.color);
                }
            }

            _blackboard.NewValueSet += OnBlackboardChanged;
        }

        private void OnBlackboardChanged(string key, string value)
        {
            if (key != RiddleConstant.FireProgress)
                return;

            if (value == RiddleConstant.Incorrect)
            {
                SetActivateCandlesticks(false).Forget();
                _blackboard.Remove(key);
            }

            if (value == RiddleConstant.Done)
            {
                ChangeColorAsync(Color.blue, 1f).Forget();
            }
        }

        public override void Interact()
        {
            SetActivateCandlesticks(true).Forget();

            _blackboard.SetValue(Key, RiddleConstant.Accept);
        }

        private async UniTask SetActivateCandlesticks(bool activate)
        {
            foreach (var fire in fires)
            {
                fire.SetActive(activate);
                await UniTask.Delay(250);
            }
        }

        private async UniTask ChangeColorAsync(Color targetColor, float duration)
        {
            var sequence = DOTween.Sequence();

            // Animate particle colors
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    var main = particles[i].main;
                    Color initialColor = main.startColor.color;

                    sequence.Join(
                        DOTween.To(
                            () => initialColor,
                            color =>
                            {
                                main.startColor = new ParticleSystem.MinMaxGradient(color);

                                var colorOverLifetime = particles[i].colorOverLifetime;
                                colorOverLifetime.enabled = true;
                                colorOverLifetime.color = flameGradient;
                            },
                            targetColor,
                            duration
                        ).SetEase(colorTransitionCurve)
                    );
                }
            }

            // Animate renderer colors
            for (int i = 0; i < renderers.Length; i++)
            {
                if (i < _originalRendererColors.Count && renderers[i] != null && renderers[i].material != null)
                {
                    sequence.Join(
                        renderers[i].material.DOColor(targetColor, duration)
                            .SetEase(colorTransitionCurve)
                            .From(_originalRendererColors[i])
                    );
                }
            }

            // Animate light colors and properties
            for (int i = 0; i < lights.Length; i++)
            {
                if (i < _originalLightColors.Count && lights[i] != null)
                {
                    Light light = lights[i];
                    float initialRange = light.range;

                    // Animate light color
                    sequence.Join(
                        light.DOColor(targetColor, duration)
                            .SetEase(colorTransitionCurve)
                    );

                    // Animate light intensity
                    sequence.Join(
                        light.DOIntensity(lightIntensity, duration)
                            .SetEase(colorTransitionCurve)
                    );

                    // Animate light range using DOTween.To since DORange doesn't exist
                    sequence.Join(
                        DOTween.To(
                            () => light.range,
                            range => light.range = range,
                            lightRange,
                            duration
                        ).SetEase(colorTransitionCurve)
                    );
                }
            }

            // Wait for all animations to complete
            await sequence.Play().AsyncWaitForCompletion();

            // Ensure final values are set exactly
            FinalizeColorChange(targetColor);
        }

        private void FinalizeColorChange(Color targetColor)
        {
            foreach (var particle in particles)
            {
                if (particle != null)
                {
                    var main = particle.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(targetColor);

                    var colorOverLifetime = particle.colorOverLifetime;
                    colorOverLifetime.enabled = true;
                    colorOverLifetime.color = flameGradient;
                }
            }

            foreach (var renderer in renderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = targetColor;
                }
            }

            // Set light colors and properties
            foreach (var light in lights)
            {
                if (light != null)
                {
                    light.color = targetColor;
                    light.intensity = lightIntensity;
                    light.range = lightRange;
                }
            }
        }
    }
}