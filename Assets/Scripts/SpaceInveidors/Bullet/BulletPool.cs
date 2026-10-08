namespace SpaceInveidors.Bullet
{
    using UnityEngine;
    using UnityEngine.Pool;

    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxSize = 30;
        [SerializeField] private bool _collectionCheck = true;

        private IObjectPool<Bullet> _pool;
        private bool _initialized;

        public bool IsInitialized => _initialized;

        private void Awake()
        {
            if (!_initialized && _bulletPrefab != null)
            {
                Initialize(
                    _bulletPrefab,
                    _defaultCapacity,
                    _maxSize,
                    _collectionCheck
                );
            }
        }

        public void Initialize(
            Bullet prefab,
            int defaultCapacity,
            int maxSize,
            bool collectionCheck)
        {
            if (_initialized)
            {
                return;
            }

            if (prefab == null)
            {
                Debug.LogError($"{nameof(BulletPool)}: bullet prefab is missing.", this);
                return;
            }

            _bulletPrefab = prefab;
            _defaultCapacity = Mathf.Max(1, defaultCapacity);
            _maxSize = Mathf.Max(_defaultCapacity, maxSize);
            _collectionCheck = collectionCheck;

            _pool = new ObjectPool<Bullet>(
                createFunc: CreateBullet,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: _collectionCheck,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );

            _initialized = true;
        }

        private Bullet CreateBullet()
        {
            if (_bulletPrefab == null)
            {
                Debug.LogError($"{nameof(BulletPool)}: bullet prefab is null.", this);
                return null;
            }

            Bullet spawnedBullet = Instantiate(_bulletPrefab, transform);
            spawnedBullet.SetPool(_pool);
            return spawnedBullet;
        }

        private void OnGetFromPool(Bullet instance)
        {
            if (instance != null)
            {
                instance.gameObject.SetActive(true);
            }
        }

        private void OnReleaseToPool(Bullet instance)
        {
            if (instance != null)
            {
                instance.gameObject.SetActive(false);
            }
        }

        private void OnDestroyPoolObject(Bullet instance)
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
        }

        public Bullet Spawn(Vector3 position, float speed, int damage)
        {
            if (!_initialized || _pool == null)
            {
                Debug.LogError($"{nameof(BulletPool)}: pool is not initialized.", this);
                return null;
            }

            Bullet bullet = _pool.Get();

            if (bullet == null)
            {
                Debug.LogError($"{nameof(BulletPool)}: spawned bullet is null.", this);
                return null;
            }

            bullet.ReUse(position, speed, damage);
            return bullet;
        }
    }
}