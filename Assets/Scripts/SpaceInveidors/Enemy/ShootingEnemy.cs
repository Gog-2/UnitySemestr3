namespace SpaceInveidors.Enemy
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using SpaceInveidors.Bullet;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(Collider2D))]
    public class ShootingEnemy : Enemy, IShootingEnemy
    {
        [Header("Shoot Settings")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _minFireInterval = 2f;
        [SerializeField] private float _maxFireInterval = 5f;
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _bulletSpeed = 5f;

        private IBulletProvider _bulletProvider;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            NormalizeIntervals();
        }

        private void OnEnable()
        {
            TryStartShooting();
        }

        private void OnDisable()
        {
            StopShooting();
        }

        protected override void OnDead()
        {
            StopShooting();
            base.OnDead();
        }

        public override void Setup(int health, int scorePerKill)
        {
            base.Setup(health, scorePerKill);
            TryStartShooting();
        }

        public void SetBulletProvider(IBulletProvider bulletProvider)
        {
            _bulletProvider = bulletProvider;
            TryStartShooting();
        }

        private void TryStartShooting()
        {
            if (!_alive) return;
            if (!isActiveAndEnabled) return;
            if (_bulletProvider == null) return;

            StartShooting();
        }

        private void StartShooting()
        {
            StopShooting();

            _cts = new CancellationTokenSource();
            ShootLoop(_cts.Token).Forget();
        }

        private void StopShooting()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void NormalizeIntervals()
        {
            _minFireInterval = Mathf.Max(0.01f, _minFireInterval);
            _maxFireInterval = Mathf.Max(_minFireInterval, _maxFireInterval);
        }

        private async UniTaskVoid ShootLoop(CancellationToken token)
        {
            float delay = Random.Range(0f, _maxFireInterval);

            bool canceled = await UniTask.WaitForSeconds(
                    delay,
                    ignoreTimeScale: false,
                    cancellationToken: token)
                .SuppressCancellationThrow();

            if (canceled) return;

            while (!token.IsCancellationRequested)
            {
                if (!_alive) break;

                if (_bulletProvider != null && _firePoint != null)
                {
                    _bulletProvider.SpawnEnemyBullet(
                        _firePoint.position,
                        _damage,
                        _bulletSpeed
                    );
                }

                delay = Random.Range(_minFireInterval, _maxFireInterval);

                canceled = await UniTask.WaitForSeconds(
                        delay,
                        ignoreTimeScale: false,
                        cancellationToken: token)
                    .SuppressCancellationThrow();

                if (canceled) break;
            }
        }
    }
}