using System;
using UnityEngine;

namespace Game.Character
{
    public class CharacterManager : MonoBehaviour, IDamageable
    {
        [Header("Vie & Armure")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxArmor = 0f;
        [SerializeField] private float resistance = 0f; // en pourcentage

        [Header("Vitesse / Effets")]
        [Tooltip("Multiplicateur de vitesse global (1 = normal, 0.5 = ralenti, 2 = rapide)")]
        [SerializeField] private float moveSpeedModifier = 1f;

        [Header("Régénération (optionnel)")]
        [SerializeField] private bool useHealthRegen = false;
        [SerializeField] private float healthRegenPerSecond = 2f;
        [SerializeField] private float regenDelayAfterHit = 3f;

        [Header("Références")]
        [SerializeField] private CharacterMovement movement;
        [SerializeField] private Animator animator;

        [Header("Animator (optionnel)")]
        [SerializeField] private string hitTriggerParam = "Hit";
        [SerializeField] private string deathTriggerParam = "Die";

        // On se base maintenant sur HealthSystem
        private HealthSystem _healthSystem;

        public float CurrentHealth => _healthSystem != null ? _healthSystem.Health : 0f;
        public float CurrentArmor => _healthSystem != null ? _healthSystem.Shield : 0f;
        public bool IsDead { get; private set; }

        public event Action<float, float> OnHealthChanged; // current, max
        public event Action<float, float> OnArmorChanged;  // current, max
        public event Action<float> OnDamageTaken;          // damage appliqué
        public event Action OnDeath;

        private float _timeSinceLastHit;
        private bool _invulnerable;

        private void Awake()
        {
            if (movement == null)
                movement = GetComponent<CharacterMovement>();
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _healthSystem = new HealthSystem(maxHealth, maxArmor, resistance);

            IsDead = false;
            _timeSinceLastHit = 0f;

            ApplySpeedModifier();
            NotifyHealthChanged();
            NotifyArmorChanged();
        }

        private void Update()
        {
            if (IsDead || _healthSystem == null) return;

            _timeSinceLastHit += Time.deltaTime;

            if (useHealthRegen && _timeSinceLastHit >= regenDelayAfterHit && CurrentHealth < maxHealth)
            {
                float oldHealth = CurrentHealth;

                float healThisFrame = healthRegenPerSecond * Time.deltaTime;
                float healPercent = (healThisFrame / maxHealth) * 100f;
                _healthSystem.HealPercent(healPercent);

                if (!Mathf.Approximately(oldHealth, CurrentHealth))
                {
                    NotifyHealthChanged();
                }
            }
        }

        public void TakeDamage(DamageInfo info)
        {
            TakeDamage(info.Amount);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || _invulnerable || amount <= 0f || _healthSystem == null)
                return;

            _timeSinceLastHit = 0f;

            float oldHealth = CurrentHealth;
            float oldArmor = CurrentArmor;

            bool died = _healthSystem.TakeDamage(amount);

            if (!Mathf.Approximately(oldArmor, CurrentArmor))
            {
                NotifyArmorChanged();
            }

            if (!Mathf.Approximately(oldHealth, CurrentHealth))
            {
                NotifyHealthChanged();
            }

            OnDamageTaken?.Invoke(amount);

            if (animator && !string.IsNullOrEmpty(hitTriggerParam) && CurrentHealth > 0f)
            {
                animator.SetTrigger(hitTriggerParam);
            }

            if (died || CurrentHealth <= 0f)
            {
                HandleDeath();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f || _healthSystem == null)
                return;

            float oldHealth = CurrentHealth;

            float healPercent = (amount / maxHealth) * 100f;
            _healthSystem.HealPercent(healPercent);

            if (!Mathf.Approximately(oldHealth, CurrentHealth))
            {
                NotifyHealthChanged();
            }
        }

        public void AddArmor(float amount)
        {
            if (amount <= 0f || _healthSystem == null) return;

            float oldArmor = CurrentArmor;
            float newArmor = Mathf.Min(maxArmor, CurrentArmor + amount);
            _healthSystem.SetShield(newArmor);

            if (!Mathf.Approximately(oldArmor, CurrentArmor))
            {
                NotifyArmorChanged();
            }
        }

        public void RemoveArmor(float amount)
        {
            if (amount <= 0f || _healthSystem == null) return;

            float oldArmor = CurrentArmor;
            float newArmor = Mathf.Max(0f, CurrentArmor - amount);
            _healthSystem.SetShield(newArmor);

            if (!Mathf.Approximately(oldArmor, CurrentArmor))
            {
                NotifyArmorChanged();
            }
        }

        public void SetInvulnerable(bool invulnerable)
        {
            _invulnerable = invulnerable;
        }

        public void SetMoveSpeedModifier(float modifier)
        {
            moveSpeedModifier = Mathf.Max(0f, modifier);
            ApplySpeedModifier();
        }

        public bool IsAlive()
        {
            return !IsDead && CurrentHealth > 0f;
        }

        private void HandleDeath()
        {
            if (IsDead) return;

            IsDead = true;

            if (movement != null)
            {
                movement.EnableMovement(false);
            }

            if (animator && !string.IsNullOrEmpty(deathTriggerParam))
            {
                animator.SetTrigger(deathTriggerParam);
            }

            OnDeath?.Invoke();
        }

        private void ApplySpeedModifier()
        {
            if (movement != null)
            {
                movement.SetSpeedModifier(moveSpeedModifier);
            }
        }

        private void NotifyHealthChanged()
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void NotifyArmorChanged()
        {
            OnArmorChanged?.Invoke(CurrentArmor, maxArmor);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
            maxArmor = Mathf.Max(0f, maxArmor);
            resistance = Mathf.Clamp(resistance, 0f, 100f);
            moveSpeedModifier = Mathf.Max(0f, moveSpeedModifier);

            if (!Application.isPlaying)
            {
                if (movement == null)
                    movement = GetComponent<CharacterMovement>();
                if (animator == null)
                    animator = GetComponent<Animator>();
            }
        }
#endif
    }
}
