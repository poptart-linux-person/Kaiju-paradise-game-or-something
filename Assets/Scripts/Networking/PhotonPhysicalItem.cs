using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace KaijuGame.Networking
{
#if PHOTON_UNITY_NETWORKING
    [RequireComponent(typeof(PhotonView))]
    public sealed class PhotonPhysicalItem : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private float interpolationSpeed = 14f;

        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Vector3 networkVelocity;
        private Vector3 networkAngularVelocity;
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }

        private void Update()
        {
            if (photonView.IsMine) return;
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * interpolationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * interpolationSpeed);
            if (body != null && !body.isKinematic)
            {
                body.velocity = networkVelocity;
                body.angularVelocity = networkAngularVelocity;
            }
        }

        public void RequestOwnership()
        {
            if (!photonView.IsMine)
                photonView.RequestOwnership();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(body != null ? body.velocity : Vector3.zero);
                stream.SendNext(body != null ? body.angularVelocity : Vector3.zero);
            }
            else
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                networkVelocity = (Vector3)stream.ReceiveNext();
                networkAngularVelocity = (Vector3)stream.ReceiveNext();
            }
        }
    }
#else
    public sealed class PhotonPhysicalItem : MonoBehaviour
    {
        public void RequestOwnership() { }
    }
#endif
}
