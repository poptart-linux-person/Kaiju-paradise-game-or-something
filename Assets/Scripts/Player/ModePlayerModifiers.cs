using System.Reflection;
using UnityEngine;
using KaijuGame.Core;

namespace KaijuGame.Player
{
    public sealed class ModePlayerModifiers : MonoBehaviour
    {
        [SerializeField] private float extractionHealthBonus = 50f;
        [SerializeField] private float extractionSpeedMultiplier = 1.35f;
        [SerializeField] private float nullSpeedMultiplier = 1.6f;

        private PlayerVitals vitals;
        private float baseMaxJumpSpeed = -1f;
        private float baseVelocityLimit = -1f;
        private float baseJumpMultiplier = -1f;
        private Component gorillaPlayer;
        private GameModeId lastMode = GameModeId.FreeRoam;
        private bool initialized;

        private void Awake()
        {
            vitals = GetComponent<PlayerVitals>();
            ResolveGorilla();
        }

        private void Update()
        {
            var manager = GameModeManager.Instance;
            if (manager == null) return;

            var mode = manager.ActiveMode;
            if (mode == lastMode && initialized) return;
            lastMode = mode;
            initialized = true;
            Apply(mode);
        }

        private void ResolveGorilla()
        {
            var type = System.Type.GetType("GorillaLocomotion.Player, Assembly-CSharp");
            if (type == null) return;
            foreach (var c in GetComponentsInChildren<Component>(true))
            {
                if (c != null && type.IsInstanceOfType(c))
                {
                    gorillaPlayer = c;
                    break;
                }
            }
        }

        private void Apply(GameModeId mode)
        {
            RestoreGorilla();
            if (vitals != null && mode == GameModeId.Extraction)
                vitals.SetTemporaryMaxHealthBonus(extractionHealthBonus, true);

            var multiplier = mode == GameModeId.Extraction ? extractionSpeedMultiplier
                : mode == GameModeId.NullBoss ? nullSpeedMultiplier
                : 1f;
            ApplyGorillaMultiplier(multiplier);
        }

        private void ApplyGorillaMultiplier(float multiplier)
        {
            if (gorillaPlayer == null || multiplier <= 0f) return;
            SetFloat(gorillaPlayer, "maxJumpSpeed", multiplier, ref baseMaxJumpSpeed);
            SetFloat(gorillaPlayer, "velocityLimit", multiplier, ref baseVelocityLimit);
            SetFloat(gorillaPlayer, "jumpMultiplier", multiplier, ref baseJumpMultiplier);
        }

        private void SetFloat(Component target, string fieldName, float multiplier, ref float original)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null || field.FieldType != typeof(float)) return;
            if (original < 0f) original = (float)field.GetValue(target);
            field.SetValue(target, original * multiplier);
        }

        private void RestoreGorilla()
        {
            if (gorillaPlayer == null) return;
            RestoreFloat(gorillaPlayer, "maxJumpSpeed", baseMaxJumpSpeed);
            RestoreFloat(gorillaPlayer, "velocityLimit", baseVelocityLimit);
            RestoreFloat(gorillaPlayer, "jumpMultiplier", baseJumpMultiplier);
        }

        private static void RestoreFloat(Component target, string fieldName, float original)
        {
            if (original < 0f) return;
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null && field.FieldType == typeof(float))
                field.SetValue(target, original);
        }
    }
}
