namespace SpaceInveidors.Enemy
{
    using System;
    using System.Collections.Generic;
    using SpaceInveidors.Bullet;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public class EnemiesFactory : IDisposable
    {
        private readonly Enemy[] _prefabsEnemies;

        private readonly int _baseVerticalAmount;
        private readonly int _baseHorizontalAmount;

        private readonly float _xOffSet;
        private readonly float _yOffSet;

        private readonly int _verticalPerWave;
        private readonly int _horizontalPerWave;
        private readonly int _healthPerWave;
        private readonly int _scorePerWave;

        private readonly int _maxVerticalAmount;
        private readonly int _maxHorizontalAmount;

        private readonly Transform _parent;
        private readonly EnemiesMover _enemiesMover;
        private readonly IBulletProvider _bulletProvider;
        private readonly List<Enemy> _enemies;

        public EnemiesFactory(
            SOEnemiesFactory soEnemiesFactory,
            Transform parent,
            EnemiesMover enemiesMover,
            IBulletProvider bulletProvider)
        {
            _prefabsEnemies = soEnemiesFactory.PrefabsEnemies;

            _baseVerticalAmount = soEnemiesFactory.VerticalAmmout;
            _baseHorizontalAmount = soEnemiesFactory.HorizontalAmmout;

            _xOffSet = soEnemiesFactory.XOffSet;
            _yOffSet = soEnemiesFactory.YOffSet;

            _verticalPerWave = soEnemiesFactory.VerticalPerWave;
            _horizontalPerWave = soEnemiesFactory.HorizontalPerWave;
            _healthPerWave = soEnemiesFactory.HealthPerWave;
            _scorePerWave = soEnemiesFactory.ScorePerWave;

            _maxVerticalAmount = soEnemiesFactory.MaxVerticalAmount;
            _maxHorizontalAmount = soEnemiesFactory.MaxHorizontalAmount;

            _parent = parent;
            _enemiesMover = enemiesMover;
            _bulletProvider = bulletProvider;

            _enemies = new List<Enemy>(
                Mathf.Max(1, _baseHorizontalAmount * _baseVerticalAmount)
            );
        }

        public void Dispose()
        {
            ClearEnemies();
        }

        public void ClearEnemies()
        {
            foreach (Enemy enemy in _enemies)
            {
                if (enemy != null)
                {
                    Object.Destroy(enemy.gameObject);
                }
            }

            _enemies.Clear();
        }

        public void SpawnWave(int wave)
        {
            ClearEnemies();

            if (_prefabsEnemies == null || _prefabsEnemies.Length == 0)
            {
                Debug.LogError("EnemiesFactory: no enemy prefabs assigned.");
                return;
            }

            wave = Mathf.Max(1, wave);

            int maxHorizontal = _maxHorizontalAmount > 0
                ? _maxHorizontalAmount
                : _baseHorizontalAmount;

            int maxVertical = _maxVerticalAmount > 0
                ? _maxVerticalAmount
                : _baseVerticalAmount;

            int horizontal = Mathf.Clamp(
                _baseHorizontalAmount + (wave - 1) * _horizontalPerWave,
                1,
                maxHorizontal
            );

            int vertical = Mathf.Clamp(
                _baseVerticalAmount + (wave - 1) * _verticalPerWave,
                1,
                maxVertical
            );

            for (int column = 0; column < horizontal; column++)
            {
                for (int row = 0; row < vertical; row++)
                {
                    Enemy prefab = _prefabsEnemies[Random.Range(0, _prefabsEnemies.Length)];

                    if (prefab == null) continue;

                    Vector3 offset = new Vector3(
                        column * _xOffSet,
                        -row * _yOffSet,
                        0f
                    );

                    Enemy spawnedEnemy = Object.Instantiate(
                        prefab,
                        _parent.position + offset,
                        Quaternion.identity,
                        _parent
                    );

                    if (spawnedEnemy == null) continue;

                    if (spawnedEnemy is IShootingEnemy shootingEnemy)
                    {
                        shootingEnemy.SetBulletProvider(_bulletProvider);
                    }

                    int health = spawnedEnemy.Health + (wave - 1) * _healthPerWave;
                    int score = spawnedEnemy.ScorePerKill + (wave - 1) * _scorePerWave;

                    spawnedEnemy.Setup(health, score);

                    _enemies.Add(spawnedEnemy);
                }
            }

            if (_enemies.Count == 0)
            {
                Debug.LogError("EnemiesFactory: wave was not spawned.");
                return;
            }

            _enemiesMover.Initialize(_enemies, _enemies.Count);
        }
    }
}