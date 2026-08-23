using UnityEngine;

namespace KaijuGame.Player
{
    // Kept as a compatibility component for scenes that already reference PlayerController.
    // Actual movement is handled by GorillaLocomotion.Player after the Gorilla package is installed.
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float bossEncounterSpeedMultiplier = 1.5f;
        public float BossEncounterSpeedMultiplier => bossEncounterSpeedMultiplier;

        public void SetBossBoost(bool active) { }
    }
}
