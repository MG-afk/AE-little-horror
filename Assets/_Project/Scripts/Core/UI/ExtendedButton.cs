using AE.Core.Systems.Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AE.Core.UI
{
    [AddComponentMenu("AE/UI/Extended Button")]
    public class ExtendedButton : Button
    {
        [SerializeField] private AudioSystem audioSystem;
        [SerializeField] private SoundType hoverSoundType = SoundType.ButtonHover;
        [SerializeField] private SoundType clickSoundType = SoundType.ButtonClick;

        [SerializeField] private bool useHoverEffects = true;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private float hoverScaleFactor = 1.05f;
        [SerializeField] private float hoverTransitionDuration = 0.1f;

        [SerializeField] private bool useClickAnimation = true;
        [SerializeField] private float clickScaleReduction = 0.95f;
        [SerializeField] private float clickAnimationDuration = 0.1f;

        private Vector3 _originalScale;
        private Image _buttonImage;
        private Sequence _currentSequence;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = transform.localScale;
            _buttonImage = GetComponent<Image>();

            if (normalColor == Color.white && _buttonImage != null)
            {
                normalColor = _buttonImage.color;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ResetAppearance();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            KillTweens();
        }

        private void KillTweens()
        {
            _currentSequence?.Kill();
            _currentSequence = null;
            DOTween.Kill(transform);
            DOTween.Kill(_buttonImage);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (!useHoverEffects || !interactable)
                return;

            KillTweens();

            _currentSequence = DOTween.Sequence();

            _currentSequence.Join(transform.DOScale(_originalScale * hoverScaleFactor, hoverTransitionDuration));

            if (_buttonImage != null)
            {
                _currentSequence.Join(_buttonImage.DOColor(hoverColor, hoverTransitionDuration));
            }

            if (audioSystem != null)
            {
                audioSystem.PlaySound(hoverSoundType);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (!useHoverEffects || !interactable)
                return;

            KillTweens();

            _currentSequence = DOTween.Sequence();

            _currentSequence.Join(transform.DOScale(_originalScale, hoverTransitionDuration));

            if (_buttonImage != null)
            {
                _currentSequence.Join(_buttonImage.DOColor(normalColor, hoverTransitionDuration));
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (!interactable)
                return;

            if (audioSystem != null)
            {
                audioSystem.PlaySound(clickSoundType);
            }

            if (!useClickAnimation)
                return;

            KillTweens();

            _currentSequence = DOTween.Sequence();

            _currentSequence.Append(transform.DOScale(_originalScale * clickScaleReduction,
                clickAnimationDuration * .5f));

            var endScale = eventData.pointerEnter == gameObject
                ? _originalScale * hoverScaleFactor
                : _originalScale;

            _currentSequence.Append(transform.DOScale(endScale, clickAnimationDuration * .5f));
        }

        private void ResetAppearance()
        {
            KillTweens();
            transform.localScale = _originalScale;

            if (_buttonImage != null)
            {
                _buttonImage.color = normalColor;
            }
        }
    }
}