using AE.SimplifyDialogue;
using UnityEngine;

namespace AE.Core.Utility
{
    public class Utilities : MonoBehaviour
    {
        [field: SerializeField] public CanvasGroup FadeCanvasGroup { get; private set; }
        [field: SerializeField] public SimplifyDialogueView SimplifyDialogueView { get; private set; }
        [field: SerializeField] public Crosshair Crosshair { get; private set; }
    }
}