using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AE.Interactions.Objects
{
    public class Candlesticks : InteractableItem
    {
        [SerializeField] private GameObject[] _candlesticks;

        public override string Text => "Interacted with Candlesticks WOW!!";

        public override void Interact()
        {
            ActiveCandlesticks().Forget();
        }

        private async UniTask ActiveCandlesticks()
        {
            foreach (var candlestick in _candlesticks)
            {
                candlestick.SetActive(true);
                await UniTask.Delay(500);
            }
        }
    }
}