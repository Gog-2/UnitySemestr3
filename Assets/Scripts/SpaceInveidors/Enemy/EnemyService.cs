namespace SpaceInveidors.Enemy
{
    using SpaceInveidors.Bullet;
    using UnityEngine;

    public class EnemyService : MonoBehaviour
    {
        [Header("Enemies Factory Settings")]
        [SerializeField] private SOEnemiesFactory _soEnemiesFactory;

        [Header("Enemies Mover Settings")]
        [SerializeField] private SOEnemiesMover _soEnemiesMover;
        [SerializeField] private Transform _leftWall;
        [SerializeField] private Transform _rightWall;

        [Header("Wave Settings")]
        [SerializeField] private WaveAnnouncer _waveAnnouncer;
        [SerializeField] private bool _announceFirstWave = true;
        [SerializeField] private bool _autoRestartAfterGameOver = false;

        private EnemiesMover _enemiesMover;
        private EnemiesFactory _enemiesFactory;

        private int _currentWave = 1;
        private bool _gameOver;

        private void Awake()
        {
            _enemiesMover = new EnemiesMover(
                transform,
                _leftWall,
                _rightWall,
                _soEnemiesMover
            );

            IBulletProvider bulletProvider = BulletService.Instance;

            _enemiesFactory = new EnemiesFactory(
                _soEnemiesFactory,
                transform,
                _enemiesMover,
                bulletProvider
            );

            _enemiesMover.OnAllEnemiesCleared += HandleAllEnemiesCleared;
        }

        private void Start()
        {
            if (GameService.Instance != null)
            {
                _currentWave = GameService.Instance.CurrentWave;
                GameService.Instance.OnGameOver += HandleGameOver;
            }
            else
            {
                Debug.LogWarning("EnemyService: GameService not found.", this);
            }

            StartWave(_currentWave, _announceFirstWave);
        }

        public void StartWave(int wave, bool announce)
        {
            if (_gameOver) return;

            _currentWave = Mathf.Max(1, wave);
            GameService.Instance?.SetCurrentWave(_currentWave);

            if (announce && _waveAnnouncer != null)
            {
                _waveAnnouncer.Announce($"Wave {_currentWave}", () => SpawnWave(_currentWave));
            }
            else
            {
                SpawnWave(_currentWave);
            }
        }

        private void SpawnWave(int wave)
        {
            if (_gameOver) return;

            _enemiesFactory.SpawnWave(wave);
        }

        private void HandleAllEnemiesCleared()
        {
            if (_gameOver) return;

            StartWave(_currentWave + 1, true);
        }

        private void HandleGameOver()
        {
            if (_gameOver) return;

            _gameOver = true;

            _enemiesMover?.OnGameClose();
            _enemiesFactory?.ClearEnemies();

            if (_waveAnnouncer != null)
            {
                _waveAnnouncer.Announce("GAME OVER", () =>
                {
                    if (_autoRestartAfterGameOver)
                    {
                        GameService.Instance?.RestartGame();
                    }
                });
            }
            else if (_autoRestartAfterGameOver)
            {
                GameService.Instance?.RestartGame();
            }
        }

        private void OnDestroy()
        {
            if (_enemiesMover != null)
            {
                _enemiesMover.OnGameClose();
                _enemiesMover.OnAllEnemiesCleared -= HandleAllEnemiesCleared;
            }

            if (GameService.Instance != null)
            {
                GameService.Instance.OnGameOver -= HandleGameOver;
            }

            _enemiesFactory?.Dispose();
        }
    }
}