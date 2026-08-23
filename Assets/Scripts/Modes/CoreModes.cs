using UnityEngine;
using KaijuGame.Core;

namespace KaijuGame.Modes
{
    public sealed class StoryMode : GameMode
    {
        public override GameModeId Id => GameModeId.Story;
    }

    public sealed class PvEMode : GameMode
    {
        public override GameModeId Id => GameModeId.PvE;
        [SerializeField] private int targetEnemyCount = 10;
        public int TargetEnemyCount => targetEnemyCount;
    }

    public sealed class ExtractionMode : GameMode
    {
        public override GameModeId Id => GameModeId.Extraction;

        [Header("Catacombs Escape")]
        [SerializeField] private float playerSpeedMultiplier = 1.35f;
        [SerializeField] private int bonusHealth = 50;
        [SerializeField] private float hunterSpeed = 11f;
        [SerializeField] private string mapId = "Catacombs";
        [SerializeField] private string exitDoorId = "CatacombsEscapeDoor";

        private bool escapeDoorFound;
        private bool escaped;
        private bool failed;

        public float PlayerSpeedMultiplier => playerSpeedMultiplier;
        public int BonusHealth => bonusHealth;
        public float HunterSpeed => hunterSpeed;
        public string MapId => mapId;
        public string ExitDoorId => exitDoorId;
        public bool EscapeDoorFound => escapeDoorFound;
        public bool Escaped => escaped;
        public bool Failed => failed;

        public override void OnModeStarted()
        {
            escapeDoorFound = false;
            escaped = false;
            failed = false;
            Debug.Log($"Extraction started on {mapId}: find {exitDoorId} while fast hunter AI pursues the players. No time limit.");
        }

        public override void OnModeEnded() { }

        public void SetEscapeDoorFound()
        {
            if (!failed && !escaped)
                escapeDoorFound = true;
        }

        public void TryEscape(bool playerInsideDoorZone)
        {
            if (failed || !escapeDoorFound || !playerInsideDoorZone) return;
            escaped = true;
            Debug.Log("Extraction successful: players escaped the Catacombs.");
        }

        public void FailExtraction()
        {
            if (escaped) return;
            failed = true;
            Debug.Log("Extraction failed: the squad was overwhelmed.");
        }
    }

    public sealed class SurvivalMode : GameMode
    {
        public override GameModeId Id => GameModeId.Survival;
        [SerializeField] private float waveLength = 45f;
        public int Wave { get; private set; } = 1;
        private float waveTimer;

        private void Update()
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= waveLength)
            {
                waveTimer = 0f;
                Wave++;
            }
        }
    }

    public sealed class InfectionMode : GameMode
    {
        public override GameModeId Id => GameModeId.Infection;
        public void Infect() => Debug.Log("Infection spread event queued.");
    }

    public sealed class TeamBattleMode : GameMode
    {
        public override GameModeId Id => GameModeId.TeamBattle;
        [SerializeField] private int scoreToWin = 50;
        public int ScoreToWin => scoreToWin;
    }

    public sealed class FreeRoamMode : GameMode
    {
        public override GameModeId Id => GameModeId.FreeRoam;
    }

    public sealed class NullBossMode : GameMode
    {
        public override GameModeId Id => GameModeId.NullBoss;
        [SerializeField] private Transform boss;
        [SerializeField] private float normalBossSpeed = 11f;
        [SerializeField] private float boostedBossSpeed = 20f;
        [SerializeField] private float playerSpeedMultiplier = 1.6f;
        [SerializeField] private float encounterDuration = 180f;
        [SerializeField] private AnimationCurve speedCurve;

        private float elapsed;
        private bool encounterActive;

        public float PlayerSpeedMultiplier => playerSpeedMultiplier;
        public float BossSpeed => Mathf.Lerp(normalBossSpeed, boostedBossSpeed, EvaluateSpeedCurve(Mathf.Clamp01(elapsed / encounterDuration)));
        public bool IsActive => encounterActive;

        public override void OnModeStarted()
        {
            elapsed = 0f;
            encounterActive = true;
            Debug.Log("NULL encounter started: players receive a major speed boost while NULL hunts them.");
        }

        public override void OnModeEnded() => encounterActive = false;

        private void Update()
        {
            if (!encounterActive || boss == null) return;
            elapsed += Time.deltaTime;

            var target = FindClosestPlayerPosition();
            if (!target.HasValue) return;

            var direction = target.Value - boss.position;
            if (direction.sqrMagnitude > 0.1f)
                boss.position += direction.normalized * BossSpeed * Time.deltaTime;
        }

        private float EvaluateSpeedCurve(float t)
        {
            return speedCurve == null || speedCurve.length == 0
                ? Mathf.Lerp(1f, 1.35f, t)
                : speedCurve.Evaluate(t);
        }

        private Vector3? FindClosestPlayerPosition()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) return null;
            if (boss == null) return players[0].transform.position;

            Vector3 closest = players[0].transform.position;
            var best = (closest - boss.position).sqrMagnitude;
            for (int i = 1; i < players.Length; i++)
            {
                var candidate = players[i].transform.position;
                var distance = (candidate - boss.position).sqrMagnitude;
                if (distance < best)
                {
                    best = distance;
                    closest = candidate;
                }
            }
            return closest;
        }
    }
}
