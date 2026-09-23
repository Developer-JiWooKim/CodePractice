using System;
using UnityEngine;

namespace Assets.MyAsset.Scripts
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private int _currentHealth;
        public int CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            private set
            {
                if (!IsDead)
                {
                    return;
                }

                int temp = _currentHealth;
                _currentHealth = Mathf.Max(_currentHealth + value, 0);

                if (temp != _currentHealth)
                {
                    OnChangeHealth?.Invoke(_currentHealth, _maxHealth);
                }

                if (_currentHealth <= 0)
                {
                    IsDead = true;
                    OnDead?.Invoke();
                }
            }
        }

        public bool IsDead { get; private set; }

        public event Action<int, int> OnChangeHealth;
        public event Action OnDead;

        private void Awake()
        {
            IsDead = false;
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0)
            {
                return;
            }
            CurrentHealth = -amount;
        }

        public void Heal(int amount)
        {
            if (amount < 0)
            {
                return;
            }

            CurrentHealth = amount;
        }
    }
}

