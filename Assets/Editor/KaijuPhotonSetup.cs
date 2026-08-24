#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KaijuGame.EditorTools
{
    public static class KaijuPhotonSetup
    {
        private const string PunAppId = "60b42ebb-61ca-4f64-8b63-a1b266167508";
        private const string VoiceAppId = "bfb8c8b6-2ab8-430e-b4d1-e116aa77ad99";

        [MenuItem("Kaiju Game/Configure Photon PUN + Voice")]
        public static void Configure()
        {
            bool pun = ConfigurePun();
            bool voice = ConfigureVoice();
            Debug.Log($"Kaiju Photon setup: PUN={(pun ? "configured" : "SDK not imported")}, Voice={(voice ? "configured" : "SDK not imported")}.\nPUN and Voice use separate App IDs.");
        }

        private static bool ConfigurePun()
        {
            var settingsType = FindType("Photon.Pun.PhotonServerSettings, Assembly-CSharp");
            if (settingsType == null) return false;

            var instanceProp = settingsType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var settings = instanceProp?.GetValue(null) as UnityEngine.Object;
            if (settings == null) return false;

            var appSettingsField = settingsType.GetField("AppSettings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var appSettings = appSettingsField?.GetValue(settings);
            if (appSettings == null) return false;

            var appIdField = appSettings.GetType().GetField("AppIdRealtime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (appIdField == null) return false;
            appIdField.SetValue(appSettings, PunAppId);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            return true;
        }

        private static bool ConfigureVoice()
        {
            var settingsType = FindType("Photon.Voice.Unity.VoiceAppSettings, Assembly-CSharp");
            if (settingsType == null)
                settingsType = FindType("Photon.Voice.AppSettings, Assembly-CSharp");
            if (settingsType == null) return false;

            var instanceProp = settingsType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var settings = instanceProp?.GetValue(null) as UnityEngine.Object;
            if (settings == null) return false;

            var appIdField = settingsType.GetField("AppIdVoice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (appIdField == null) return false;
            appIdField.SetValue(settings, VoiceAppId);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            return true;
        }

        private static Type FindType(string name)
        {
            var type = Type.GetType(name);
            if (type != null) return type;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(name.Replace(", Assembly-CSharp", ""));
                if (type != null) return type;
            }
            return null;
        }
    }
}
#endif
