using UnityEngine;
using UnityEngine.Pool;

namespace Nuclear_reactor
{
    public class CoinPool : MonoBehaviour
    {
        [SerializeField] private Coin _coinPrefab;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxSize = 30;
        [SerializeField] private bool _collectionCheck = true;

        private ObjectPool<Coin> _pool;
        private bool _initialized;

        public bool IsInitialized => _initialized;
        private bool _canSpawn => _pool.CountActive < _maxSize;
        
        private void Awake()
        {

            Initialize(
                _coinPrefab,
                _defaultCapacity,
                _maxSize,
                _collectionCheck
                );

        }

        public void Initialize(
            Coin prefab,
            int defaultCapacity,
            int maxSize,
            bool collectionCheck)
        {
            if (_initialized)
            {
                return;
            }
            
            _coinPrefab = prefab;
            _defaultCapacity = Mathf.Max(1, defaultCapacity);
            _maxSize = Mathf.Max(_defaultCapacity, maxSize);
            _collectionCheck = collectionCheck;

            _pool = new ObjectPool<Coin>(
                createFunc: CreateCoin,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: _collectionCheck,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );

            _initialized = true;
        }

        private Coin CreateCoin()
        {

            Coin spawnedCoin = Instantiate(_coinPrefab);
            spawnedCoin.SetPool(_pool);
            return spawnedCoin;
        }

        private void OnGetFromPool(Coin instance)
        {
            instance.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(Coin instance)
        {
            instance.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Coin instance)
        {
            Destroy(instance.gameObject);
        }

        public bool TrySpawn(Vector3 position)
        {
            if (!_canSpawn) return false;
            
            Coin coin = _pool.Get();
            coin.ReUse(position);
            return true;
        }
    }
}
