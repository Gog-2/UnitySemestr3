namespace SpaceInveidors.Player.Component
{
    using Cysharp.Threading.Tasks;
    using SpaceInveidors.Bullet;
    using SpaceInveidors.Sound;
    using UnityEngine;

    public class Shoter
    {
        private readonly IBulletProvider _bulletProvider;
        private readonly ISoundProvider _soundProvider;
        private readonly Transform _shootPoint;
        private readonly int _damage;
        private readonly float _speed;
        private readonly float _shotCooldown;

        private bool _canShoot = true;

        public Shoter(
            Transform shootPoint,
            IBulletProvider bulletProvider,
            int damage,
            float speed,
            float shotCooldown)
        {
            _shootPoint = shootPoint;
            _bulletProvider = bulletProvider;
            _damage = damage;
            _speed = speed;
            _shotCooldown = shotCooldown;
        }

        public void Shoot()
        {
            if (!_canShoot || _bulletProvider == null || _shootPoint == null)
            {
                return;
            }

            _soundProvider?.PlayPlayerShot();

            _bulletProvider.SpawnPlayerBullet(
                _shootPoint.position,
                _damage,
                _speed
            );

            _canShoot = false;
            ShootCooldownAsync().Forget();
        }

        private async UniTaskVoid ShootCooldownAsync()
        {
            await UniTask.WaitForSeconds(_shotCooldown);
            _canShoot = true;
        }
    }
}
