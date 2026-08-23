using UnityEngine;
using UnityEngine.XR;

namespace KaijuGame.Items
{
    public sealed class PhysicalItemHandInput : MonoBehaviour
    {
        [SerializeField] private PhysicalItemHand hand;
        [SerializeField] private XRNode node = XRNode.LeftHand;
        [SerializeField] private float throwVelocityScale = 1.2f;
        [SerializeField] private float throwAngularScale = 0.5f;

        private InputDevice device;
        private bool previousGrip;
        private bool previousTrigger;

        private void Awake()
        {
            if (hand == null) hand = GetComponent<PhysicalItemHand>();
            RefreshDevice();
        }

        private void Update()
        {
            if (!device.isValid) RefreshDevice();
            if (!device.isValid || hand == null) return;

            device.TryGetFeatureValue(CommonUsages.gripButton, out var grip);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out var trigger);

            if (grip && !previousGrip)
                hand.TryGrab();
            if (!grip && previousGrip && hand.HeldItem != null)
            {
                var velocity = Vector3.zero;
                var angular = Vector3.zero;
                device.TryGetFeatureValue(CommonUsages.deviceVelocity, out velocity);
                device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out angular);
                hand.Release(velocity * throwVelocityScale, angular * throwAngularScale);
            }
            if (trigger && !previousTrigger)
                hand.Use(transform.root.gameObject);

            previousGrip = grip;
            previousTrigger = trigger;
        }

        private void RefreshDevice()
        {
            device = InputDevices.GetDeviceAtXRNode(node);
        }
    }
}
