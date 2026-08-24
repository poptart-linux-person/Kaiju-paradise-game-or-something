using System;
using UnityEngine;

namespace KaijuGame.Player
{
    public class PlayerVitals : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegen = 18f;

        private float baseMaxHealth;

        public float Health { get; private set; }
        public float MaxHealth => maxHealth;
        public float Stamina { get; private set; }
        public bool IsDowned { get; private set; }
        public bool IsDead { get; private set; }
        public event Action<float> HealthChanged;
        public event Action<bool> DownedChanged;
        public event Action<bool> DeathChanged;

        private void Awake()
        {
            baseMaxHealth = maxHealth;
            Health = maxHealth;
            Stamina = maxStamina;
        }

        private void Update()
        {
            if (!IsDowned && !IsDead)
                Stamina = Mathf.Min(maxStamina, Stamina + staminaRegen * Time.deltaTime);
        }

        public void Damage(float amount)
        {
            if (IsDowned || IsDead || amount <= 0f) return;
            Health = Mathf.Max(0f, Health - amount);
            HealthChanged?.Invoke(Health);
            if (Health <= 0f) SetDowned(true);
        }

        public void TakeDamage(float amount) => Damage(amount);

        public void Heal(float amount)
        {
            if (IsDowned || IsDead || amount <= 0f) return;
            Health = Mathf.Min(maxHealth, Health + amount);
            HealthChanged?.Invoke(Health);
        }

        public bool TryRevive(float healthPercent = 0.35f)
        {
            if (!IsDowned || IsDead) return false;
            IsDowned = false;
            Health = Mathf.Clamp(maxHealth * healthPercent, 1f, maxHealth);
            Stamina = Mathf.Min(maxStamina, maxStamina * 0.5f);
            HealthChanged?.Invoke(Health);
            DownedChanged?.Invoke(false);
            return true;
        }

        public void Kill()
        {
            IsDead = true;
            IsDowned = false;
            Health = 0f;
            HealthChanged?.Invoke(Health);
            DownedChanged?.Invoke(false);
            DeathChanged?.Invoke(true);
        }

        public void ResetForRespawn()
        {
            IsDead = false;
            IsDowned = false;
            Health = maxHealth;
            Stamina = maxStamina;
            HealthChanged?.Invoke(Health);
            DownedChanged?.Invoke(false);
            DeathChanged?.Invoke(false);
        }

        public void SetTemporaryMaxHealthBonus(float bonus, bool refill = true)
        {
            maxHealth = Mathf.Max(1f, baseMaxHealth + bonus);
            Health = refill ? maxHealth : Mathf.Min(Health, maxHealth);
            HealthChanged?.Invoke(Health);
        }

        public void ClearTemporaryMaxHealthBonus(bool restoreBaseHealth = true)
        {
            maxHealth = baseMaxHealth;
            Health = restoreBaseHealth ? maxHealth : Mathf.Min(Health, maxHealth);
            HealthChanged?.Invoke(Health);
        }

        public bool TrySpendStamina(float amount)
        {
            if (IsDowned || IsDead || amount <= 0f || Stamina < amount) return false;
            Stamina -= amount;
            return true;
        }

        public void SetDowned(bool downed)
        {
            if (IsDead) return;
            IsDowned = downed;
            DownedChanged?.Invoke(downed);
        }
    }
}
