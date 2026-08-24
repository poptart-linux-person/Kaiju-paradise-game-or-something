using UnityEngine;

namespace KaijuGame.Player
{
    public sealed class PlayerReviveInteractable : MonoBehaviour
    {
        [SerializeField] private float reviveRange = 1.75f;
        [SerializeField] private float reviveDuration = 2.5f;
        [SerializeField] private float reviveHealthPercent = 0.35f;

        private float progress;
        private PlayerVitals target;

        public float Progress01 => reviveDuration <= 0f ? 1f : Mathf.Clamp01(progress / reviveDuration);

        private void Update()
        {
            var localVitals = GetComponentInParent<PlayerVitals>();
            if (localVitals == null || localVitals.IsDowned || localVitals.IsDead)
            {
                ResetProgress();
                return;
            }

            target = FindClosestDownedTeammate();
            if (target == null)
            {
                ResetProgress();
                return;
            }

            // Hold an interaction button from your input layer by calling SetReviving.
            if (!reviving)
            {
                ResetProgress();
                return;
            }

            progress += Time.deltaTime;
            if (progress >= reviveDuration && target.TryRevive(reviveHealthPercent))
                ResetProgress();
        }

        private bool reviving;

        public void SetReviving(bool active) => reviving = active;

        private PlayerVitals FindClosestDownedTeammate()
        {
            var best = reviveRange * reviveRange;
            PlayerVitals result = null;
            foreach (var candidate in FindObjectsOfType<PlayerVitals>())
            {
                if (candidate == null || !candidate.IsDowned || candidate.IsDead) continue;
                var distance = (candidate.transform.position - transform.position).sqrMagnitude;
                if (distance <= best)
                {
                    best = distance;
                    result = candidate;
                }
            }
            return result;
        }

        private void ResetProgress()
        {
            progress = 0f;
            target = null;
        }
    }
}
