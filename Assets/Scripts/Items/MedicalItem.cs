using UnityEngine;
using KaijuGame.Player;

namespace KaijuGame.Items
{
    public sealed class MedicalItem : PhysicalItem
    {
        public enum MedicalType { Medkit, Bandage }

        [SerializeField] private MedicalType medicalType = MedicalType.Medkit;
        [SerializeField] private float healAmount = 60f;
        [SerializeField] private float useTime = 1.25f;
        [SerializeField] private bool consumeOnUse = true;

        private float useTimer;
        private GameObject currentUser;
        private bool usingItem;

        public float RemainingUseTime => Mathf.Max(0f, useTimer);
        public bool IsUsing => usingItem;

        private void Update()
        {
            if (!usingItem || currentUser == null) return;
            useTimer -= Time.deltaTime;
            if (useTimer > 0f) return;
            FinishUse();
        }

        public override bool Use(GameObject user)
        {
            if (usingItem || user == null) return false;
            var vitals = user.GetComponentInParent<PlayerVitals>();
            if (vitals == null || vitals.IsDowned) return false;
            if (vitals.Health >= vitals.MaxHealth) return false;

            currentUser = user;
            usingItem = true;
            useTimer = Mathf.Max(0.05f, useTime);
            return true;
        }

        private void FinishUse()
        {
            var vitals = currentUser != null ? currentUser.GetComponentInParent<PlayerVitals>() : null;
            if (vitals != null)
                vitals.Heal(healAmount);

            usingItem = false;
            currentUser = null;
            if (consumeOnUse)
                Destroy(gameObject);
        }
    }
}
