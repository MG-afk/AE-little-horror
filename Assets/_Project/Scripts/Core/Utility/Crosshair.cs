using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AE.Core.Utility
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private Image crosshairImage;
        [SerializeField] private float hoverScale = 1.3f;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private Ease animationEase = Ease.OutBack;

        private bool _isHovering;
        private Tweener _currentTween;

        private void Start()
        {
            crosshairImage.transform.localScale = Vector3.one;
        }

        public void Hover()
        {
            if (_isHovering) return;

            _isHovering = true;
            AnimateTo(hoverScale);
        }

        public void Unhover()
        {
            if (!_isHovering) return;

            _isHovering = false;
            AnimateTo(1f);
        }

        private void AnimateTo(float targetScale)
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill(false);
            }

            _currentTween = crosshairImage.transform
                .DOScale(Vector3.one * targetScale, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true);
        }

        private void OnDestroy()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill(false);
            }
        }
    }
}