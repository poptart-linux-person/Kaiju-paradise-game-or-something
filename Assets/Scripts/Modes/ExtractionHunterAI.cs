using UnityEngine;

namespace KaijuGame.Modes
{
    public sealed class ExtractionHunterAI : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float chaseRange = 45f;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float attackCooldown = 1f;

        private float currentSpeed;
        private float attackTimer;
        private Transform target;

        private void Update()
        {
            attackTimer -= Time.deltaTime;
            target = FindClosestPlayer();
            if (target == null) return;

            var offset = target.position - transform.position;
            var distance = offset.magnitude;
            if (distance > chaseRange) return;

            if (distance > attackRange)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
                transform.position += offset.normalized * currentSpeed * Time.deltaTime;
                transform.forward = Vector3.Slerp(transform.forward, offset.normalized, Time.deltaTime * 8f);
            }
            else if (attackTimer <= 0f)
            {
                attackTimer = attackCooldown;
                target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        private Transform FindClosestPlayer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            Transform closest = null;
            var best = chaseRange * chaseRange;
            foreach (var player in players)
            {
                var distance = (player.transform.position - transform.position).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                    closest = player.transform;
                }
            }
            return closest;
        }
    }
}
