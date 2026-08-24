#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace KaijuGame.EditorTools
{
    public static class ChaseAudioAutoSetup
    {
        [MenuItem("Kaiju Game/Auto-Setup Chase Audio")]
        public static void Setup()
        {
            const string folder = "Assets/Audio/ProceduralChase";
            var ambient = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/Chase_Base_Drone.asset");
            var chase = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/Chase_Pulse.asset");
            var danger = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/Chase_Percussion.asset");
            var panic = AssetDatabase.LoadAssetAtPath<AudioClip>($"{folder}/Chase_Panic_Lead.asset");

            var go = GameObject.Find("ChaseAudio") ?? new GameObject("ChaseAudio");
            var bootstrap = go.GetComponent<KaijuGame.Audio.AutoChaseAudioBootstrap>() ?? go.AddComponent<KaijuGame.Audio.AutoChaseAudioBootstrap>();
            var serialized = new SerializedObject(bootstrap);
            serialized.FindProperty("ambient").objectReferenceValue = ambient;
            serialized.FindProperty("chase").objectReferenceValue = chase;
            serialized.FindProperty("danger").objectReferenceValue = danger;
            serialized.FindProperty("panic").objectReferenceValue = panic;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(go);
            Selection.activeGameObject = go;
            Debug.Log("Chase audio auto-setup complete. PvE/Extraction threats are picked up automatically at runtime.");
        }
    }
}
#endif
