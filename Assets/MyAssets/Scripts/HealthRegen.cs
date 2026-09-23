using System;
using System.Threading;
using UnityEngine;

namespace Assets.MyAsset.Scripts
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthRegen : MonoBehaviour
    {
        [SerializeField] private int _regenPerSecond = 10;
        [SerializeField] private float _regenDelay = 3f;

        private Health _health;

        private CancellationTokenSource _cts;

        private int _healthCache;

        private bool _isDamaged;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthChanged += HealtChagnedEventHelper;

            _healthCache = _health.CurrentHealth;

            StartRegen(); // 활성화 되면 체력을 갱신, 리젠을 시작
        }

        private void OnDisable()
        {
            _health.OnHealthChanged -= HealtChagnedEventHelper;
        }

        private void HealtChagnedEventHelper(int currentHealth, int maxHealth)
        {
            // 체력 변화 이벤트가 발생했지만, 그것이 체력 회복인지, 데미지 입은것인지 모르므로 여기서 판별
            // 데미지를 입었으면 작동중인 Regen을 멈추는 로직(bool변수를 활용할듯)
            // DelayRegenTimer() 작동 시켜서 3초간 대기하도록
            // 대기 중 또 데미지를 입으면 이 타이머를 다시 초기화하는 메소드 필요한데 여기서 토큰을 쓰면 될듯
            if (_healthCache > currentHealth)
            {
                _healthCache = currentHealth;
                // 이전에 저장해둔 체력값이 최근 받아온 값보다 크면 데미지를 입은것이 확실
                _isDamaged = true; // 데미지 입은것 여부, 이건 Delaytimer가 끝나면 false로 되돌려야됨

                DelayRegenTimer(_regenDelay);
            }
            else if (_healthCache <= currentHealth)
            {
                // 이전 저장해둔 체력보다 현재 체력이 높으면 힐을 한것이므로 체력 갱신만
                _healthCache = currentHealth;
            }

        }

        private void StartRegen()
        {
            if (_health.IsDead)
            {
                _cts.Cancel();
                return;
            }
            _isDamaged = true;
            DelayRegenTimer(_regenDelay);
        }

        private async void DelayRegenTimer(float delaytime)
        {
            _cts.Cancel();
            Cancel();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            try
            {
                await Awaitable.WaitForSecondsAsync(3f, _cts.Token);


                Regen(); // 딜레이 동안 
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                _isDamaged = false;
            }


        }

        private async void Regen()
        {
            _cts.Cancel();
            Cancel();

            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            try
            {
                while (true)
                {
                    // 데미지를 입기전까지
                    await Awaitable.WaitForSecondsAsync(1, destroyCancellationToken);
                    _health.Heal(_regenPerSecond);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private void OnDestroy()
        {
            Cancel();
        }

        private void Cancel()
        {
            if (_cts == null)
            {
                return;
            }
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
