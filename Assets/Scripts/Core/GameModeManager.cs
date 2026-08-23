using System;
using System.Collections.Generic;
using UnityEngine;

namespace KaijuGame.Core
{
    public sealed class GameModeManager : MonoBehaviour
    {
        public static GameModeManager Instance { get; private set; }
        [SerializeField] private GameModeId startupMode = GameModeId.FreeRoam;
        private readonly Dictionary<GameModeId, GameMode> modes = new();
        private GameMode activeMode;

        public GameModeId ActiveMode => activeMode != null ? activeMode.Id : startupMode;
        public event Action<GameModeId> ModeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CacheModes();
        }

        public void StartMode(GameModeId id)
        {
            CacheModes();
            if (!modes.TryGetValue(id, out var next))
            {
                Debug.LogWarning($"No GameMode component registered for {id}.");
                return;
            }

            activeMode?.OnModeEnded();
            activeMode = next;
            activeMode.OnModeStarted();
            ModeChanged?.Invoke(id);
        }

        private void CacheModes()
        {
            modes.Clear();
            foreach (var mode in FindObjectsOfType<GameMode>(true))
                modes[mode.Id] = mode;
        }
    }
}
