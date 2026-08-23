using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using Photon.Realtime;
#endif

namespace KaijuGame.Networking
{
    public class PhotonLauncher : MonoBehaviour
#if PHOTON_UNITY_NETWORKING
        , IConnectionCallbacks, IMatchmakingCallbacks
#endif
    {
        [SerializeField] private string gameVersion = "0.1.0";
        [SerializeField] private byte maxPlayersPerRoom = 8;
        private const string PhotonAppId = "60b42ebb-61ca-4f64-8b63-a1b266167508";

        private void Start()
        {
#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PhotonAppId;
            PhotonNetwork.ConnectUsingSettings();
#else
            Debug.Log("Photon PUN 2 is not installed yet. Import PUN 2, then enable PHOTON_UNITY_NETWORKING in your scripting define symbols.");
#endif
        }

        public void CreateRoom(string roomCode)
        {
#if PHOTON_UNITY_NETWORKING
            roomCode = string.IsNullOrWhiteSpace(roomCode) ? CreateCode() : roomCode.Trim().ToUpperInvariant();
            PhotonNetwork.CreateRoom(roomCode, new RoomOptions { MaxPlayers = maxPlayersPerRoom, IsVisible = false, IsOpen = true });
#else
            Debug.LogWarning($"CreateRoom requested: {roomCode}. Photon is not installed.");
#endif
        }

        public void JoinRoom(string roomCode)
        {
#if PHOTON_UNITY_NETWORKING
            roomCode = roomCode?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(roomCode)) return;
            PhotonNetwork.JoinRoom(roomCode);
#else
            Debug.LogWarning($"JoinRoom requested: {roomCode}. Photon is not installed.");
#endif
        }

        public void QuickJoin()
        {
#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.JoinRandomRoom();
#else
            Debug.LogWarning("QuickJoin requested. Photon is not installed.");
#endif
        }

        private static string CreateCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var code = new char[5];
            for (var i = 0; i < code.Length; i++) code[i] = chars[Random.Range(0, chars.Length)];
            return new string(code);
        }

#if PHOTON_UNITY_NETWORKING
        public void OnConnected() { }
        public void OnConnectedToMaster() => Debug.Log("Photon connected to Master Server.");
        public void OnDisconnected(DisconnectCause cause) => Debug.LogWarning($"Photon disconnected: {cause}");
        public void OnRegionListReceived(RegionHandler regionHandler) { }
        public void OnCustomAuthenticationResponse(System.Collections.Generic.Dictionary<string, object> data) { }
        public void OnCustomAuthenticationFailed(string debugMessage) => Debug.LogWarning(debugMessage);
        public void OnFriendListUpdate(System.Collections.Generic.List<FriendInfo> friendList) { }
        public void OnCreatedRoom() => Debug.Log("Photon room created.");
        public void OnCreateRoomFailed(short returnCode, string message) => Debug.LogWarning($"Create room failed: {returnCode} {message}");
        public void OnJoinedRoom() => Debug.Log("Joined Photon room.");
        public void OnJoinRoomFailed(short returnCode, string message) => Debug.LogWarning($"Join room failed: {returnCode} {message}");
        public void OnJoinRandomFailed(short returnCode, string message) => Debug.LogWarning($"Quick join failed: {returnCode} {message}");
        public void OnLeftRoom() { }
#endif
    }
}
