using UnityEngine;

namespace KaijuGame.Voice
{
    public sealed class VoiceHearingAI : MonoBehaviour
    {
        [SerializeField] private float hearingRange = 30f;
        [SerializeField] private float minimumVoiceLevel = 0.08f;
        [SerializeField] private float memorySeconds = 2.5f;
        [SerializeField] private float scanInterval = 0.1f;
        [SerializeField] private bool requireLineOfSight = false;
        [SerializeField] private LayerMask sightMask = ~0;

        private Transform heardTarget;
        private VoiceMeterNetwork[] meters = System.Array.Empty<VoiceMeterNetwork>();
        private float memoryTimer;
        private float scanTimer;

        public Transform HeardTarget => memoryTimer > 0f ? heardTarget : null;
        public bool HeardSomething => HeardTarget != null;

        private void Update()
        {
            memoryTimer -= Time.deltaTime;
            scanTimer -= Time.deltaTime;
            if (scanTimer <= 0f)
            {
                scanTimer = Mathf.Max(0.02f, scanInterval);
                ScanForVoice();
            }

            if (memoryTimer <= 0f)
                heardTarget = null;
        }

        private void ScanForVoice()
        {
            meters = FindObjectsOfType<VoiceMeterNetwork>();
            var bestScore = 0f;
            Transform bestTarget = null;

            foreach (var meter in meters)
            {
                if (meter == null || !meter.IsSpeaking) continue;
                var offset = meter.transform.position - transform.position;
                var distance = offset.magnitude;
                if (distance > hearingRange || meter.VoiceLevel < minimumVoiceLevel) continue;

                if (requireLineOfSight)
                {
                    var origin = transform.position + Vector3.up * 0.5f;
                    if (Physics.Raycast(origin, offset.normalized, distance, sightMask, QueryTriggerInteraction.Ignore))
                        continue;
                }

                var score = meter.VoiceLevel * (1f - distance / hearingRange);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = meter.transform;
                }
            }

            if (bestTarget != null)
            {
                heardTarget = bestTarget;
                memoryTimer = memorySeconds;
            }
        }
    }
}
