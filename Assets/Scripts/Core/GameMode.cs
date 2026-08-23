using UnityEngine;

namespace KaijuGame.Core
{
    public enum GameModeId
    {
        Story,
        PvE,
        Extraction,
        Survival,
        Infection,
        TeamBattle,
        FreeRoam,
        NullBoss
    }

    public abstract class GameMode : MonoBehaviour
    {
        public abstract GameModeId Id { get; }
        public virtual void OnModeStarted() { }
        public virtual void OnModeEnded() { }
    }
}
