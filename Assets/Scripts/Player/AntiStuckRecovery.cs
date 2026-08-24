using UnityEngine;

namespace KaijuGame.Player
{
    public sealed class AntiStuckRecovery : MonoBehaviour
    {
        [SerializeField] private float fallThreshold = -30f;
        [SerializeField] private float minimumGroundedDistance = 0.4f;
        [SerializeField] private float recoveryCooldown = 2f;
        [SerializeField] private LayerMask groundMask = ~0;

        private Vector3 lastSafePosition;
        private float cooldown;

        private void Start()
        {
            lastSafePosition = transform.position;
        }

        private void FixedUpdate()
        {
            cooldown -= Time.fixedDeltaTime;
            if (cooldown > 0f) return;

            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out _, minimumGroundedDistance, groundMask, QueryTriggerInteraction.Ignore))
                lastSafePosition = transform.position;

            if (transform.position.y < fallThreshold)
            {
                var body = GetComponent<Rigidbody>();
                if (body != null)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                    body.position = lastSafePosition;
                }
                else
                {
                    transform.position = lastSafePosition;
                }
                cooldown = recoveryCooldown;
            }
        }
    }
}
