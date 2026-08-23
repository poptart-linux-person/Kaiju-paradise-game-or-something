using UnityEngine;
using KaijuGame.World;

namespace KaijuGame.Items
{
    public sealed class CrowbarItem : PhysicalItem
    {
        [SerializeField] private float pryForce = 22f;
        [SerializeField] private float hitSpeedRequired = 3f;

        private void OnCollisionEnter(Collision collision)
        {
            var door = collision.collider.GetComponentInParent<SecurityDoor>();
            if (door == null || collision.relativeVelocity.magnitude < hitSpeedRequired) return;

            var impulse = collision.relativeVelocity.normalized * pryForce;
            door.Break(impulse);
        }

        public override bool Use(GameObject user)
        {
            if (user == null) return false;
            var origin = user.transform.position;
            var direction = user.transform.forward;
            if (!Physics.Raycast(origin, direction, out var hit, 2f)) return false;
            var door = hit.collider.GetComponentInParent<SecurityDoor>();
            if (door == null) return false;
            door.Break(direction * pryForce);
            return true;
        }
    }
}
