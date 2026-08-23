using UnityEngine;

namespace KaijuGame.Networking
{
    // Keeps the project voice-ready without hard-depending on Photon Voice before its SDK is imported.
    public sealed class PhotonVoiceBootstrap : MonoBehaviour
    {
        public const string VoiceAppId = "bfb8c8b6-2ab8-430e-b4d1-e116aa77ad99";

        private void Awake()
        {
            ApplyVoiceAppId();
        }

        public static void ApplyVoiceAppId()
        {
#if PHOTON_VOICE
            var settingsType = System.Type.GetType("Photon.Voice.PUN.PhotonVoiceSettings, PhotonVoice.PUN");
            if (settingsType != null)
            {
                var settings = Object.FindFirstObjectByType(settingsType);
                if (settings == null)
                    Debug.Log("Photon Voice SDK detected. Configure its App Settings to use the Kaiju Game Voice App ID.");
            }
#else
            Debug.Log("Photon Voice App ID configured in KaijuGame.Networking.PhotonVoiceBootstrap. Import Photon Voice to enable live voice chat.");
#endif
        }
    }
}
