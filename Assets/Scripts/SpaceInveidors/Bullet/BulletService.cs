// ============================================================
// BulletService.cs
// ============================================================

namespace SpaceInveidors.Bullet
{
    using UnityEngine;

    [DefaultExecutionOrder(-100)]
    public class BulletService : MonoBehaviour, IBulletProvider
    {
        public static BulletService Instance { get; private set; }
        
        private BulletPool _playerBulletPool;
        
        private BulletPool _enemyBulletPool;

        [Header("Player Bullet Prefab")]
        [SerializeField] private Bullet _playerBulletPrefab;
        [SerializeField] private int _playerDefaultCapacity = 10;
        [SerializeField] private int _playerMaxSize = 30;
        [SerializeField] private bool _playerCollectionCheck = true;

        [Header("Enemy Bullet Prefab")]
        [SerializeField] private Bullet _enemyBulletPrefab;
        [SerializeField] private int _enemyDefaultCapacity = 10;
        [SerializeField] private int _enemyMaxSize = 30;
        [SerializeField] private bool _enemyCollectionCheck = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            EnsurePools();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        private void EnsurePools()
        {
            _playerBulletPool = ResolvePool(
                _playerBulletPool,
                "PlayerBulletPool",
                _playerBulletPrefab,
                _playerDefaultCapacity,
                _playerMaxSize,
                _playerCollectionCheck
            );

            _enemyBulletPool = ResolvePool(
                _enemyBulletPool,
                "EnemyBulletPool",
                _enemyBulletPrefab,
                _enemyDefaultCapacity,
                _enemyMaxSize,
                _enemyCollectionCheck
            );
        }

        private BulletPool ResolvePool(
            BulletPool existingPool,
            string poolName,
            Bullet prefab,
            int defaultCapacity,
            int maxSize,
            bool collectionCheck)
        {
            if (existingPool != null)
            {
                if (!existingPool.IsInitialized && prefab != null)
                {
                    existingPool.Initialize(
                        prefab,
                        defaultCapacity,
                        maxSize,
                        collectionCheck
                    );
                }

                if (!existingPool.IsInitialized)
                {
                    Debug.LogError($"BulletService: {poolName} exists but is not initialized.", this);
                }

                return existingPool;
            }

            if (prefab == null)
            {
                Debug.LogError($"BulletService: {poolName} prefab is missing.", this);
                return null;
            }

            GameObject poolObject = new GameObject(poolName);
            poolObject.transform.SetParent(transform, false);

            BulletPool pool = poolObject.AddComponent<BulletPool>();

            pool.Initialize(
                prefab,
                defaultCapacity,
                maxSize,
                collectionCheck
            );

            return pool;
        }

        public void SpawnPlayerBullet(Vector3 position, int damage, float speed)
        {
            if (_playerBulletPool == null || !_playerBulletPool.IsInitialized)
            {
                Debug.LogError("BulletService: PlayerBulletPool is missing or not initialized.", this);
                return;
            }

            _playerBulletPool.Spawn(position, speed, damage);
        }

        public void SpawnEnemyBullet(Vector3 position, int damage, float speed)
        {
            if (_enemyBulletPool == null || !_enemyBulletPool.IsInitialized)
            {
                Debug.LogError("BulletService: EnemyBulletPool is missing or not initialized.", this);
                return;
            }

            _enemyBulletPool.Spawn(position, speed, damage);
        }
    }
}