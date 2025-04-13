using AE.Core.Utility;
using Cysharp.Threading.Tasks;
using VContainer;

namespace AE.Interactions.Objects
{
    public class TextOnWall : InteractableItem
    {
        private const string TextOnInteraction = "What does that mean...?\nIs this some kind of warning?";

        private Utilities _utilities;

        [Inject]
        private void Construct(Utilities utilities)
        {
            _utilities = utilities;
        }

        public void Awake()
        {
            gameObject.SetActive(false);
        }

        public override void Interact()
        {
            _utilities.SimplifyDialogueView.Show(TextOnInteraction).Forget();
        }
    }
}