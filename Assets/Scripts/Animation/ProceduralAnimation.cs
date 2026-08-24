using System.Collections;
using UnityEngine;

namespace KaijuGame.Animation
{
    /// <summary>
    /// Procedural foot/leg placement for creature rigs. Designed to work with
    /// physics-driven movement without owning the locomotion system.
    /// </summary>
    public sealed class ProceduralAnimation : MonoBehaviour
    {
        [Header("Leg Targets")]
        [SerializeField] private Transform[] legTargets = System.Array.Empty<Transform>();
        [SerializeField] private float stepSize = 0.65f;
        [SerializeField] private float stepHeight = 0.12f;
        [SerializeField] private float raycastRange = 1.25f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Motion")]
        [SerializeField] private float velocityInfluence = 7f;
        [SerializeField] private int smoothness = 4;
        [SerializeField] private bool orientBodyToFeet;
        [SerializeField] private float bodyOrientationSpeed = 8f;

        private Vector3[] defaultLocalPositions = System.Array.Empty<Vector3>();
        private Vector3[] lastLegPositions = System.Array.Empty<Vector3>();
        private bool[] legMoving = System.Array.Empty<bool>();
        private Vector3 lastBodyPosition;
        private Vector3 smoothedVelocity;
        private Vector3 lastBodyUp;

        private void Awake()
        {
            if (legTargets == null)
                legTargets = System.Array.Empty<Transform>();

            var count = legTargets.Length;
            defaultLocalPositions = new Vector3[count];
            lastLegPositions = new Vector3[count];
            legMoving = new bool[count];

            for (var i = 0; i < count; i++)
            {
                if (legTargets[i] == null)
                    continue;

                defaultLocalPositions[i] = legTargets[i].localPosition;
                lastLegPositions[i] = legTargets[i].position;
            }

            lastBodyPosition = transform.position;
            lastBodyUp = transform.up;
        }

        private void FixedUpdate()
        {
            if (legTargets.Length == 0)
                return;

            var rawVelocity = (transform.position - lastBodyPosition) / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
            smoothedVelocity = Vector3.Lerp(smoothedVelocity, rawVelocity, 0.35f);
            lastBodyPosition = transform.position;

            var desired = new Vector3[legTargets.Length];
            var indexToMove = -1;
            var furthestDistance = stepSize;

            for (var i = 0; i < legTargets.Length; i++)
            {
                var target = legTargets[i];
                if (target == null)
                    continue;

                desired[i] = transform.TransformPoint(defaultLocalPositions[i]) + Vector3.ProjectOnPlane(smoothedVelocity, transform.up) * velocityInfluence * Time.fixedDeltaTime;
                var planarDistance = Vector3.ProjectOnPlane(desired[i] - lastLegPositions[i], transform.up).magnitude;

                if (!legMoving[i] && planarDistance > furthestDistance)
                {
                    furthestDistance = planarDistance;
                    indexToMove = i;
                }
            }

            for (var i = 0; i < legTargets.Length; i++)
            {
                var target = legTargets[i];
                if (target == null || i == indexToMove || legMoving[i])
                    continue;

                target.position = lastLegPositions[i];
            }

            if (indexToMove >= 0)
            {
                var placement = FindGroundPlacement(desired[indexToMove], transform.up);
                if (placement.hit)
                    StartCoroutine(Step(indexToMove, placement.point));
            }

            if (orientBodyToFeet && legTargets.Length >= 4)
                UpdateBodyOrientation();
        }

        private IEnumerator Step(int index, Vector3 targetPoint)
        {
            if (index < 0 || index >= legTargets.Length || legTargets[index] == null)
                yield break;

            legMoving[index] = true;
            var start = lastLegPositions[index];
            var steps = Mathf.Max(1, smoothness);

            for (var i = 1; i <= steps; i++)
            {
                var t = i / (float)steps;
                var eased = t * t * (3f - 2f * t);
                var position = Vector3.Lerp(start, targetPoint, eased);
                position += transform.up * Mathf.Sin(t * Mathf.PI) * stepHeight;
                legTargets[index].position = position;
                yield return new WaitForFixedUpdate();
            }

            lastLegPositions[index] = targetPoint;
            legTargets[index].position = targetPoint;
            legMoving[index] = false;
        }

        private (bool hit, Vector3 point, Vector3 normal) FindGroundPlacement(Vector3 point, Vector3 up)
        {
            var origin = point + up * raycastRange * 0.5f;
            if (Physics.Raycast(origin, -up, out var hit, raycastRange, groundMask, QueryTriggerInteraction.Ignore))
                return (true, hit.point, hit.normal);

            return (false, point, up);
        }

        private void UpdateBodyOrientation()
        {
            var v1 = legTargets[0] != null && legTargets[1] != null ? legTargets[0].position - legTargets[1].position : Vector3.zero;
            var v2 = legTargets[2] != null && legTargets[3] != null ? legTargets[2].position - legTargets[3].position : Vector3.zero;
            var normal = Vector3.Cross(v1, v2).normalized;
            if (normal.sqrMagnitude < 0.001f)
                return;

            lastBodyUp = Vector3.Slerp(lastBodyUp, normal, bodyOrientationSpeed * Time.fixedDeltaTime);
            transform.up = lastBodyUp;
        }

        public void ResetFooting()
        {
            StopAllCoroutines();
            for (var i = 0; i < legMoving.Length; i++)
            {
                legMoving[i] = false;
                if (legTargets[i] != null)
                    legTargets[i].position = lastLegPositions[i];
            }
        }
    }
}
