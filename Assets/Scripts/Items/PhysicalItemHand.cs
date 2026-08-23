using UnityEngine;
using KaijuGame.Networking;

namespace KaijuGame.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PhysicalItemHand : MonoBehaviour
    {
        [SerializeField] private float searchRadius = 0.35f;
        [SerializeField] private LayerMask itemLayers = ~0;

        private Rigidbody body;
        private PhysicalItem heldItem;

        public PhysicalItem HeldItem => heldItem;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        public bool TryGrab()
        {
            if (heldItem != null) return false;
            var hits = Physics.OverlapSphere(transform.position, searchRadius, itemLayers, QueryTriggerInteraction.Ignore);
            PhysicalItem nearest = null;
            var best = float.MaxValue;
            foreach (var hit in hits)
            {
                var item = hit.GetComponentInParent<PhysicalItem>();
                if (item == null || item.IsHeld) continue;
                var distance = (item.transform.position - transform.position).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                    nearest = item;
                }
            }

            if (nearest == null) return false;
            nearest.GetComponent<PhotonPhysicalItem>()?.RequestOwnership();
            heldItem = nearest;
            heldItem.Grab(transform, body);
            return true;
        }

        public void Release(Vector3 throwVelocity, Vector3 throwAngularVelocity)
        {
            if (heldItem == null) return;
            var item = heldItem;
            heldItem = null;
            item.Throw(throwVelocity, throwAngularVelocity);
        }

        public bool Use(GameObject user)
        {
            return heldItem != null && heldItem.Use(user);
        }

        public void Drop()
        {
            if (heldItem == null) return;
            var item = heldItem;
            heldItem = null;
            item.Release();
        }
    }
}
