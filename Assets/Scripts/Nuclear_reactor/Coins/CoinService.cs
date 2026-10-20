using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nuclear_reactor
{
    public class CoinService : Singltone<CoinService>
    {
        [SerializeField] private float _spawnColdownBase;
        [SerializeField] private float _stepForColdown;
        [SerializeField] private CoinPool _pool;
        [SerializeField] private Transform _coinMinPos, _coinMaxPos;
        
        [SerializeField] 
        [ReadOnly]private float _spawnCol;
        
        private Vector3 _minPos;
        private Vector3 _maxPos;
        private Vector3 _randomSpawn;
        
        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();

            _spawnCol = _spawnColdownBase;
            
            UpgradeSpawnSpeed(UpgradeType.SpawnSpeed, 
                Inventory.Instance.Upgrades[UpgradeType.SpawnSpeed]);

            Inventory.Instance.OnUpgradeChanged += UpgradeSpawnSpeed;
            
            _minPos = _coinMinPos.position;
            _maxPos = _coinMaxPos.position;
            
            _cts = new CancellationTokenSource();
            
            Run(_cts.Token).Forget();
        }
        
        private async UniTaskVoid Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool isCanceled = await UniTask.WaitForSeconds(
                        _spawnCol,
                        ignoreTimeScale: false,
                        cancellationToken: token)
                    .SuppressCancellationThrow();
                
                if (isCanceled) break;
                
                SpawnCoin();
            }
        }

        private void SpawnCoin()
        {
            _pool.TrySpawn(RandomSpawn());
        }
        private Vector3 RandomSpawn()
        {
            return new Vector3(
                Random.Range(Mathf.Min(_minPos.x, _maxPos.x), Mathf.Max(_minPos.x, _maxPos.x)),
                Random.Range(Mathf.Min(_minPos.y, _maxPos.y), Mathf.Max(_minPos.y, _maxPos.y)),
                Random.Range(Mathf.Min(_minPos.z, _maxPos.z), Mathf.Max(_minPos.z, _maxPos.z)));
        }

        private void UpgradeSpawnSpeed(UpgradeType upgradeType, int value)
        {
            if (upgradeType == UpgradeType.SpawnSpeed)
            {
                _spawnCol = _spawnColdownBase - _stepForColdown * (value - 1);
            }
        }

        private void OnDisable()
        {
            Inventory.Instance.OnUpgradeChanged -= UpgradeSpawnSpeed;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
