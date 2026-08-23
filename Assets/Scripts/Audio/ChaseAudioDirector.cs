using UnityEngine;

namespace KaijuGame.Audio
{
    public sealed class ChaseAudioDirector : MonoBehaviour
    {
        [SerializeField] private AudioSource ambientLayer;
        [SerializeField] private AudioSource chaseLayer;
        [SerializeField] private AudioSource dangerLayer;
        [SerializeField] private Transform listenerTarget;
        [SerializeField] private float chaseDistance = 35f;
        [SerializeField] private float dangerDistance = 10f;
        [SerializeField] private float fadeSpeed = 4f;

        private Transform[] threats;

        public void SetThreats(Transform[] activeThreats) => threats = activeThreats;

        private void Update()
        {
            if (listenerTarget == null) return;
            var nearest = FindNearestThreatDistance();
            var chase01 = nearest <= chaseDistance ? 1f - Mathf.InverseLerp(0f, chaseDistance, nearest) : 0f;
            var danger01 = nearest <= dangerDistance ? 1f - Mathf.InverseLerp(0f, dangerDistance, nearest) : 0f;

            Fade(ambientLayer, 1f - chase01 * 0.35f);
            Fade(chaseLayer, chase01);
            Fade(dangerLayer, danger01);
        }

        private float FindNearestThreatDistance()
        {
            if (threats == null || threats.Length == 0) return float.PositiveInfinity;
            var best = float.PositiveInfinity;
            foreach (var threat in threats)
            {
                if (threat == null) continue;
                best = Mathf.Min(best, Vector3.Distance(listenerTarget.position, threat.position));
            }
            return best;
        }

        private void Fade(AudioSource source, float target)
        {
            if (source == null) return;
            source.volume = Mathf.MoveTowards(source.volume, Mathf.Clamp01(target), fadeSpeed * Time.deltaTime);
        }
    }
}
