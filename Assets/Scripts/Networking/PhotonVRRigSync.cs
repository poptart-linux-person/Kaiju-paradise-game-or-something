using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace KaijuGame.Networking
{
#if PHOTON_UNITY_NETWORKING
    public sealed class PhotonVRRigSync : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [SerializeField] private float interpolationSpeed = 18f;

        private Vector3 remoteHeadPosition;
        private Quaternion remoteHeadRotation;
        private Vector3 remoteLeftPosition;
        private Quaternion remoteLeftRotation;
        private Vector3 remoteRightPosition;
        private Quaternion remoteRightRotation;

        private void Awake()
        {
            remoteHeadPosition = head != null ? head.localPosition : Vector3.zero;
            remoteHeadRotation = head != null ? head.localRotation : Quaternion.identity;
            remoteLeftPosition = leftHand != null ? leftHand.localPosition : Vector3.zero;
            remoteLeftRotation = leftHand != null ? leftHand.localRotation : Quaternion.identity;
            remoteRightPosition = rightHand != null ? rightHand.localPosition : Vector3.zero;
            remoteRightRotation = rightHand != null ? rightHand.localRotation : Quaternion.identity;
        }

        private void Update()
        {
            if (photonView.IsMine) return;
            ApplyRemote(head, remoteHeadPosition, remoteHeadRotation);
            ApplyRemote(leftHand, remoteLeftPosition, remoteLeftRotation);
            ApplyRemote(rightHand, remoteRightPosition, remoteRightRotation);
        }

        private void ApplyRemote(Transform target, Vector3 position, Quaternion rotation)
        {
            if (target == null) return;
            target.localPosition = Vector3.Lerp(target.localPosition, position, Time.deltaTime * interpolationSpeed);
            target.localRotation = Quaternion.Slerp(target.localRotation, rotation, Time.deltaTime * interpolationSpeed);
        }

        public void SetTargets(Transform headTarget, Transform leftHandTarget, Transform rightHandTarget)
        {
            head = headTarget;
            leftHand = leftHandTarget;
            rightHand = rightHandTarget;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                WriteTransform(stream, head);
                WriteTransform(stream, leftHand);
                WriteTransform(stream, rightHand);
            }
            else
            {
                ReadTransform(stream, ref remoteHeadPosition, ref remoteHeadRotation);
                ReadTransform(stream, ref remoteLeftPosition, ref remoteLeftRotation);
                ReadTransform(stream, ref remoteRightPosition, ref remoteRightRotation);
            }
        }

        private static void WriteTransform(PhotonStream stream, Transform target)
        {
            stream.SendNext(target != null ? target.localPosition : Vector3.zero);
            stream.SendNext(target != null ? target.localRotation : Quaternion.identity);
        }

        private static void ReadTransform(PhotonStream stream, ref Vector3 position, ref Quaternion rotation)
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }
#else
    public sealed class PhotonVRRigSync : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        public void SetTargets(Transform headTarget, Transform leftHandTarget, Transform rightHandTarget)
        {
            head = headTarget;
            leftHand = leftHandTarget;
            rightHand = rightHandTarget;
        }
    }
#endif
}
