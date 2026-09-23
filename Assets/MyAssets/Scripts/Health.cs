using System;
using UnityEngine;

namespace Assets.MyAsset.Scripts // 이건 내가 코드 스타일이야, 존중해줘
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private int _currentHealth;
        public int CurrentHealth => _currentHealth;

        public bool IsDead { get; private set; }

        public event Action<int, int> OnHealthChanged;
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
            ApplyDelta(-amount);
        }

        public void Heal(int amount)
        {
            if (amount < 0)
            {
                return;
            }

            ApplyDelta(amount);
        }

        private void ApplyDelta(int delta)
        {
            if (IsDead)
            {
                return;
            }

            int temp = _currentHealth;
            _currentHealth = Mathf.Clamp(_currentHealth + delta, 0, _maxHealth);

            if (temp != _currentHealth)
            {
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
                Debug.Log("체력 변화 이벤트 실행");
            }

            if (_currentHealth <= 0)
            {
                IsDead = true;
                OnDead?.Invoke();
                Debug.Log("사망 이벤트 실행");
            }
        }

        public void HpLog()
        {
            Debug.Log(CurrentHealth.ToString());
        }
    }
}

