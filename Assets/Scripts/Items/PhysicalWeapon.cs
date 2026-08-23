using UnityEngine;

namespace KaijuGame.Items
{
    public sealed class PhysicalWeapon : PhysicalItem
    {
        [SerializeField] private float damage = 25f;
        [SerializeField] private float impactForce = 8f;
        [SerializeField] private bool damageOnCollision = true;
        [SerializeField] private float collisionCooldown = 0.2f;

        private float lastHitTime = -10f;

        private void OnCollisionEnter(Collision collision)
        {
            if (!damageOnCollision || Time.time - lastHitTime < collisionCooldown) return;
            if (collision.relativeVelocity.magnitude < 2f) return;
            var target = collision.collider.GetComponentInParent<KaijuGame.Player.PlayerVitals>();
            if (target == null) return;

            lastHitTime = Time.time;
            target.Damage(damage);
            if (collision.rigidbody != null && !collision.rigidbody.isKinematic)
                collision.rigidbody.AddForce(collision.relativeVelocity.normalized * impactForce, ForceMode.Impulse);
        }

        public override bool Use(GameObject user)
        {
            var forward = transform.forward;
            var hit = Physics.SphereCast(transform.position, 0.08f, forward, out var info, 1.5f);
            if (!hit) return false;
            var target = info.collider.GetComponentInParent<KaijuGame.Player.PlayerVitals>();
            if (target == null) return false;
            target.Damage(damage);
            return true;
        }
    }
}
