using UnityEngine;

namespace AE.Core.Utility
{
    public class LightFlicker : MonoBehaviour
    {
        [SerializeField] private Light torchLight;
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.2f;
        [SerializeField] private float flickerSpeed = 0.1f;

        private void Update()
        {
            torchLight.intensity =
                Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0));
        }
    }
}