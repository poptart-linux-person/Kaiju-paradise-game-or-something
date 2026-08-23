using System;
using UnityEngine;

namespace KaijuGame.Player
{
    public class PlayerVitals : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaRegen = 18f;

        public float Health { get; private set; }
        public float Stamina { get; private set; }
        public bool IsDowned { get; private set; }
        public event Action<float> HealthChanged;
        public event Action<bool> DownedChanged;

        private void Awake()
        {
            Health = maxHealth;
            Stamina = maxStamina;
        }

        private void Update()
        {
            if (!IsDowned)
                Stamina = Mathf.Min(maxStamina, Stamina + staminaRegen * Time.deltaTime);
        }

        public void Damage(float amount)
        {
            if (IsDowned || amount <= 0f) return;
            Health = Mathf.Max(0f, Health - amount);
            HealthChanged?.Invoke(Health);
            if (Health <= 0f) SetDowned(true);
        }

        public void Heal(float amount)
        {
            if (IsDowned || amount <= 0f) return;
            Health = Mathf.Min(maxHealth, Health + amount);
            HealthChanged?.Invoke(Health);
        }

        public bool TrySpendStamina(float amount)
        {
            if (IsDowned || amount <= 0f || Stamina < amount) return false;
            Stamina -= amount;
            return true;
        }

        public void SetDowned(bool downed)
        {
            IsDowned = downed;
            DownedChanged?.Invoke(downed);
        }
    }
}
