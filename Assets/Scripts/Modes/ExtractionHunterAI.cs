using UnityEngine;
using UnityEngine.AI;
using KaijuGame.Player;
using KaijuGame.Voice;

namespace KaijuGame.Modes
{
    [RequireComponent(typeof(VoiceHearingAI))]
    public sealed class ExtractionHunterAI : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 14f;
        [SerializeField] private float acceleration = 28f;
        [SerializeField] private float chaseRange = 65f;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float attackCooldown = 0.8f;
        [SerializeField] private float stopDistance = 1.2f;
        [SerializeField] private float targetScanInterval = 0.1f;

        private float currentSpeed;
        private float attackTimer;
        private float targetScanTimer;
        private Transform target;
        private Transform[] players = System.Array.Empty<Transform>();
        private NavMeshAgent agent;
        private VoiceHearingAI hearing;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            hearing = GetComponent<VoiceHearingAI>();
            if (agent != null)
            {
                agent.speed = moveSpeed;
                agent.acceleration = acceleration;
                agent.stoppingDistance = stopDistance;
                agent.angularSpeed = 720f;
            }
        }

        private void Update()
        {
            attackTimer -= Time.deltaTime;
            targetScanTimer -= Time.deltaTime;
            if (targetScanTimer <= 0f)
            {
                targetScanTimer = Mathf.Max(0.02f, targetScanInterval);
                target = hearing != null && hearing.HeardTarget != null
                    ? hearing.HeardTarget
                    : FindClosestPlayer();
            }

            if (target == null) return;

            var offset = target.position - transform.position;
            var distance = offset.magnitude;
            if (distance > chaseRange)
            {
                if (agent != null && agent.isOnNavMesh) agent.ResetPath();
                return;
            }

            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
            }
            else if (distance > attackRange)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
                transform.position += offset.normalized * currentSpeed * Time.deltaTime;
                transform.forward = Vector3.Slerp(transform.forward, offset.normalized, Time.deltaTime * 10f);
            }

            if (distance <= attackRange && attackTimer <= 0f)
            {
                attackTimer = attackCooldown;
                target.GetComponentInParent<PlayerVitals>()?.TakeDamage(damage);
            }
        }

        private Transform FindClosestPlayer()
        {
            var objects = GameObject.FindGameObjectsWithTag("Player");
            var best = chaseRange * chaseRange;
            Transform closest = null;
            foreach (var player in objects)
            {
                if (player == null || !player.activeInHierarchy) continue;
                var distance = (player.transform.position - transform.position).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                    closest = player.transform;
                }
            }
            players = objects.Length > 0 ? System.Array.ConvertAll(objects, x => x != null ? x.transform : null) : System.Array.Empty<Transform>();
            return closest;
        }
    }
}
