#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using KaijuGame.Core;
using KaijuGame.Modes;
using KaijuGame.Networking;

namespace KaijuGame.EditorTools
{
    public static class ProjectBootstrapper
    {
        [MenuItem("Kaiju Game/Create Prototype Scene")]
        public static void CreatePrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var systems = new GameObject("GameSystems");
            systems.AddComponent<GameModeManager>();
            systems.AddComponent<PhotonLauncher>();
            systems.AddComponent<StoryMode>();
            systems.AddComponent<PvEMode>();
            systems.AddComponent<ExtractionMode>();
            systems.AddComponent<SurvivalMode>();
            systems.AddComponent<InfectionMode>();
            systems.AddComponent<TeamBattleMode>();
            systems.AddComponent<FreeRoamMode>();
            systems.AddComponent<NullBossMode>();

            var player = new GameObject("PrototypePlayer");
            player.tag = "Player";
            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.mass = 70f;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var head = new GameObject("HeadCollider");
            head.transform.SetParent(player.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var headCollider = head.AddComponent<SphereCollider>();
            headCollider.radius = 0.12f;

            var body = new GameObject("BodyCollider");
            body.transform.SetParent(player.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            var bodyCollider = body.AddComponent<CapsuleCollider>();
            bodyCollider.height = 0.9f;
            bodyCollider.radius = 0.25f;

            var leftTracking = new GameObject("LeftHandTracking").transform;
            leftTracking.SetParent(player.transform, false);
            var rightTracking = new GameObject("RightHandTracking").transform;
            rightTracking.SetParent(player.transform, false);

            var tracking = player.AddComponent<KaijuGame.Player.XRTrackingTargets>();
            var trackingSerialized = new SerializedObject(tracking);
            trackingSerialized.FindProperty("headTarget").objectReferenceValue = head.transform;
            trackingSerialized.FindProperty("leftHandTarget").objectReferenceValue = leftTracking;
            trackingSerialized.FindProperty("rightHandTarget").objectReferenceValue = rightTracking;
            trackingSerialized.ApplyModifiedPropertiesWithoutUndo();

            player.AddComponent<KaijuGame.Player.PlayerVitals>();
            player.AddComponent<KaijuGame.Player.GorillaPlayerRigBinder>();
            player.AddComponent<KaijuGame.Networking.PhotonPlayer>();

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 80f;

            var lightObject = new GameObject("Sun");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Prototype.unity");
            Selection.activeGameObject = player;
            Debug.Log("Prototype scene created. Install Gorilla Locomotion, then configure the selected model with Kaiju Game/Configure Selected Model As VR Player.");
        }
    }
}
#endif
