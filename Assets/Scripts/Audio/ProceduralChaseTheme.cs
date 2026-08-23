using UnityEngine;

namespace KaijuGame.Audio
{
    public sealed class ProceduralChaseTheme : MonoBehaviour
    {
        [SerializeField] private AudioSource[] layers;
        [SerializeField] private float minIntensity = 0f;
        [SerializeField] private float maxIntensity = 1f;
        [SerializeField] private float fadeSpeed = 3f;
        [SerializeField] private Transform threatTarget;
        [SerializeField] private float farDistance = 45f;
        [SerializeField] private float nearDistance = 3f;
        [SerializeField] private bool autoPlay = true;

        private float intensity;

        private void Start()
        {
            if (autoPlay)
            {
                foreach (var layer in layers)
                {
                    if (layer == null) continue;
                    layer.loop = true;
                    if (!layer.isPlaying) layer.Play();
                    layer.volume = 0f;
                }
            }
        }

        private void Update()
        {
            float targetIntensity = 0f;
            if (threatTarget != null)
            {
                float distance = Vector3.Distance(transform.position, threatTarget.position);
                targetIntensity = Mathf.InverseLerp(farDistance, nearDistance, distance);
            }

            intensity = Mathf.MoveTowards(intensity, Mathf.Clamp01(targetIntensity), fadeSpeed * Time.deltaTime);

            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer == null) continue;
                float layerThreshold = i * 0.25f;
                float layerGain = Mathf.InverseLerp(layerThreshold, Mathf.Min(1f, layerThreshold + 0.35f), intensity);
                layer.volume = Mathf.SmoothStep(0f, 1f, layerGain);
            }
        }

        public void SetThreatTarget(Transform target) => threatTarget = target;
        public void SetIntensity(float value) => intensity = Mathf.Clamp(value, minIntensity, maxIntensity);
    }
}
