using UnityEngine;

namespace KaijuGame.Items
{
    public enum PhysicalItemType
    {
        Generic,
        Weapon,
        Medkit,
        Bandage
    }

    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalItem : MonoBehaviour
    {
        [SerializeField] private PhysicalItemType itemType = PhysicalItemType.Generic;
        [SerializeField] private float grabMass = 1f;
        [SerializeField] private bool canBeUsedAsPhysicalSupport = true;

        private Rigidbody body;
        private Transform originalParent;
        private FixedJoint activeJoint;

        public PhysicalItemType ItemType => itemType;
        public Rigidbody Body => body;
        public bool IsHeld => activeJoint != null;
        public bool CanBeUsedAsPhysicalSupport => canBeUsedAsPhysicalSupport;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.mass = Mathf.Max(0.05f, grabMass);
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            originalParent = transform.parent;
        }

        public virtual void Grab(Transform hand, Rigidbody handBody = null)
        {
            if (hand == null || handBody == null) return;
            Release();
            transform.SetParent(null, true);
            body.isKinematic = false;
            activeJoint = gameObject.AddComponent<FixedJoint>();
            activeJoint.connectedBody = handBody;
            activeJoint.enableCollision = true;
            activeJoint.breakForce = Mathf.Infinity;
            activeJoint.breakTorque = Mathf.Infinity;
        }

        public virtual void Release()
        {
            if (activeJoint != null)
            {
                Destroy(activeJoint);
                activeJoint = null;
            }

            if (originalParent != null)
                transform.SetParent(originalParent, true);
            body.isKinematic = false;
        }

        public virtual void Throw(Vector3 velocity, Vector3 angularVelocity)
        {
            Release();
            body.velocity = velocity;
            body.angularVelocity = angularVelocity;
        }

        public virtual bool Use(GameObject user) => false;
    }
}
