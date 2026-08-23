using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace KaijuGame.Networking
{
#if PHOTON_UNITY_NETWORKING
    public sealed class PhotonPlayer : MonoBehaviourPun, IPunObservable
    {
        private Vector3 networkPosition;
        private Quaternion networkRotation;

        private void Awake()
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }

        private void Update()
        {
            if (photonView.IsMine) return;
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 12f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 12f);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
#endif
}
