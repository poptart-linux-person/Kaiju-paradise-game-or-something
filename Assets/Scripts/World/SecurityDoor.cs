using UnityEngine;
using KaijuGame.Items;

namespace KaijuGame.World
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class SecurityDoor : MonoBehaviour
    {
        [SerializeField] private KeycardLevel requiredCard = KeycardLevel.Blue;
        [SerializeField] private float breakForce = 18f;
        [SerializeField] private float breakImpactSpeed = 7f;
        [SerializeField] private float openTorque = 90f;
        [SerializeField] private bool canJumpThrough = true;
        [SerializeField] private bool autoOpenWithKeycard = true;

        private Rigidbody body;
        private bool opened;
        private bool broken;

        public bool IsOpen => opened;
        public bool IsBroken => broken;
        public KeycardLevel RequiredCard => requiredCard;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public bool TryOpen(GameObject user)
        {
            if (opened || broken || user == null) return false;
            var inventory = user.GetComponentInParent<KeycardInventory>();
            if (inventory == null || !inventory.Has(requiredCard)) return false;
            Open();
            return true;
        }

        public void Open()
        {
            if (opened || broken) return;
            opened = true;
            body.isKinematic = false;
            body.AddTorque(transform.up * openTorque, ForceMode.Impulse);
        }

        public void Break(Vector3 impulse)
        {
            if (broken) return;
            broken = true;
            opened = true;
            body.isKinematic = false;
            body.AddForce(impulse, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (broken || opened) return;

            if (autoOpenWithKeycard)
            {
                var inventory = collision.collider.GetComponentInParent<KeycardInventory>();
                if (inventory != null && inventory.Has(requiredCard))
                {
                    Open();
                    return;
                }
            }

            if (collision.relativeVelocity.magnitude >= breakImpactSpeed && collision.impulse.magnitude >= breakForce)
            {
                if (canJumpThrough && collision.collider.GetComponentInParent<Rigidbody>() != null)
                    Break(collision.impulse.normalized * Mathf.Max(1f, collision.relativeVelocity.magnitude));
            }
        }
    }
}
