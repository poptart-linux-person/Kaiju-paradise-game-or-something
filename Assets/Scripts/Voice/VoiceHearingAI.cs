using UnityEngine;

namespace KaijuGame.Voice
{
    public sealed class VoiceHearingAI : MonoBehaviour
    {
        [SerializeField] private float hearingRange = 30f;
        [SerializeField] private float minimumVoiceLevel = 0.08f;
        [SerializeField] private float memorySeconds = 2.5f;
        [SerializeField] private bool requireLineOfSight = false;
        [SerializeField] private LayerMask sightMask = ~0;

        private Transform heardTarget;
        private float memoryTimer;

        public Transform HeardTarget => memoryTimer > 0f ? heardTarget : null;
        public bool HeardSomething => HeardTarget != null;

        private void Update()
        {
            var meters = FindObjectsOfType<VoiceMeterNetwork>();
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
            else
            {
                memoryTimer -= Time.deltaTime;
                if (memoryTimer <= 0f) heardTarget = null;
            }
        }
    }
}
