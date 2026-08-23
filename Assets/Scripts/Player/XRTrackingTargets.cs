using UnityEngine;
using UnityEngine.XR;

namespace KaijuGame.Player
{
    public sealed class XRTrackingTargets : MonoBehaviour
    {
        [Header("Tracking targets used by GorillaLocomotion")]
        [SerializeField] private Transform headTarget;
        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private Transform rightHandTarget;
        [SerializeField] private float heightOffset = 0f;

        private InputDevice headDevice;
        private InputDevice leftHandDevice;
        private InputDevice rightHandDevice;

        private void Awake()
        {
            FindDevices();
        }

        private void Update()
        {
            if (!headDevice.isValid || !leftHandDevice.isValid || !rightHandDevice.isValid)
                FindDevices();

            UpdateDevice(headDevice, headTarget, heightOffset);
            UpdateDevice(leftHandDevice, leftHandTarget, 0f);
            UpdateDevice(rightHandDevice, rightHandTarget, 0f);
        }

        private void FindDevices()
        {
            headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        private static void UpdateDevice(InputDevice device, Transform target, float yOffset)
        {
            if (!device.isValid || target == null) return;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out var position))
                target.localPosition = position + Vector3.up * yOffset;

            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation))
                target.localRotation = rotation;
        }
    }
}
