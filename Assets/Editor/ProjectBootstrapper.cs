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
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;
            player.AddComponent<KaijuGame.Player.PlayerController>();
            player.AddComponent<KaijuGame.Player.PlayerVitals>();
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
            Selection.activeGameObject = systems;
            Debug.Log("Prototype scene created at Assets/Scenes/Prototype.unity");
        }
    }
}
#endif
