#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace KaijuGame.EditorTools
{
    public static class GorillaLocomotionInstaller
    {
        private const string PackageUrl = "https://github.com/Another-Axiom/GorillaLocomotion/raw/main/GorillaLocomotion.unitypackage";
        private const string LocalPackagePath = "Library/KaijuGame/GorillaLocomotion.unitypackage";
        private static UnityWebRequest request;

        [MenuItem("Kaiju Game/Install Gorilla Locomotion")]
        public static void Install()
        {
            if (System.Type.GetType("GorillaLocomotion.Player, Assembly-CSharp") != null)
            {
                Debug.Log("GorillaLocomotion.Player is already installed.");
                return;
            }

            var directory = Path.GetDirectoryName(LocalPackagePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            request = UnityWebRequest.Get(PackageUrl);
            request.SendWebRequest().completed += _ =>
            {
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Gorilla Locomotion download failed: {request.error}");
                        return;
                    }

                    File.WriteAllBytes(LocalPackagePath, request.downloadHandler.data);
                    AssetDatabase.ImportPackage(LocalPackagePath, true);
                    AssetDatabase.Refresh();
                    Debug.Log("Gorilla Locomotion imported successfully.");
                }
                finally
                {
                    request.Dispose();
                    request = null;
                }
            };
        }
    }
}
#endif
