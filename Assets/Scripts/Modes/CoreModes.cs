using System.Collections;
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
        [SerializeField] private int objectivesRequired = 3;
        [SerializeField] private float extractionHoldSeconds = 12f;
        private int completedObjectives;
        private float extractionTimer;

        public bool CanExtract => completedObjectives >= objectivesRequired;
        public float ExtractionProgress => extractionHoldSeconds <= 0f ? 1f : Mathf.Clamp01(extractionTimer / extractionHoldSeconds);

        public void CompleteObjective()
        {
            completedObjectives = Mathf.Clamp(completedObjectives + 1, 0, objectivesRequired);
        }

        public bool TickExtraction(float deltaTime, bool playersInsideZone)
        {
            if (!CanExtract || !playersInsideZone)
            {
                extractionTimer = 0f;
                return false;
            }

            extractionTimer += Mathf.Max(0f, deltaTime);
            return extractionTimer >= extractionHoldSeconds;
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
        public void Infect() { Debug.Log("Infection spread event queued."); }
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
        [SerializeField] private float normalBossSpeed = 9f;
        [SerializeField] private float boostedBossSpeed = 15f;
        [SerializeField] private float playerSpeedMultiplier = 1.5f;
        [SerializeField] private float encounterDuration = 180f;
        [SerializeField] private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1.35f);

        private float elapsed;
        private bool encounterActive;

        public float PlayerSpeedMultiplier => playerSpeedMultiplier;
        public float BossSpeed => Mathf.Lerp(normalBossSpeed, boostedBossSpeed, speedCurve.Evaluate(Mathf.Clamp01(elapsed / encounterDuration)));
        public bool IsActive => encounterActive;

        public override void OnModeStarted()
        {
            elapsed = 0f;
            encounterActive = true;
            Debug.Log("NULL encounter started: players receive a temporary speed boost.");
        }

        public override void OnModeEnded()
        {
            encounterActive = false;
        }

        private void Update()
        {
            if (!encounterActive) return;
            elapsed += Time.deltaTime;

            if (boss != null)
            {
                var t = FindClosestPlayerPosition();
                if (t.HasValue)
                {
                    var direction = (t.Value - boss.position);
                    if (direction.sqrMagnitude > 0.1f)
                        boss.position += direction.normalized * BossSpeed * Time.deltaTime;
                }
            }
        }

        private Vector3? FindClosestPlayerPosition()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) return null;

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
