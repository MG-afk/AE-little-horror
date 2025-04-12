using UnityEngine;
using TMPro;
using DG.Tweening;

public class ManipulationHintUI : MonoBehaviour
{
    [SerializeField] private GameObject holdingPanel;
    [SerializeField] private GameObject placementPanel;
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private Sequence _fadeTween;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
            
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        // Hide both panels initially
        Hide();
    }

    public void ShowHoldingHints()
    {
        holdingPanel.SetActive(true);
        placementPanel.SetActive(false);
        FadeInAndOut();
    }

    public void ShowPlacementHints()
    {
        holdingPanel.SetActive(false);
        placementPanel.SetActive(true);
        FadeInAndOut();
    }

    public void Hide()
    {
        holdingPanel.SetActive(false);
        placementPanel.SetActive(false);
        if (_fadeTween != null)
            _fadeTween.Kill();
    }

    private void FadeInAndOut()
    {
        if (_fadeTween != null)
            _fadeTween.Kill();

        canvasGroup.alpha = 0;

        _fadeTween = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, 0.5f))
            .AppendInterval(displayDuration)
            .Append(canvasGroup.DOFade(0, 1f))
            .OnComplete(() =>
            {
                holdingPanel.SetActive(false);
                placementPanel.SetActive(false);
            });
    }
}