namespace SpaceInveidors.Enemy
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEngine;

    public class EnemiesMover
    {
        private List<Enemy> _enemies;
        private readonly Transform _enemiesTransform;

        private readonly Vector3 _startPos;
        private Vector3 _targetPos;

        private readonly float _stepSize;
        private readonly float _baseCooldown;
        private readonly float _distanceNext;

        private readonly float _xLow;
        private readonly float _xMax;

        private readonly double _speedPerKillMult;

        private bool _isRightSide = true;

        private CancellationTokenSource _cts;

        private int _totalEnemies;
        private int _aliveCount;
        private float _currentCooldown;
        private bool _allClearedInvoked;

        public event Action OnAllEnemiesCleared;

        public EnemiesMover(
            Transform enemiesTransform,
            Transform leftWall,
            Transform rightWall,
            SOEnemiesMover soEnemiesMover)
        {
            _enemiesTransform = enemiesTransform;

            _startPos = enemiesTransform.position;
            _targetPos = _startPos;

            _xLow = leftWall.position.x;
            _xMax = rightWall.position.x;

            _stepSize = soEnemiesMover.stepSize;
            _baseCooldown = soEnemiesMover.coldown;
            _distanceNext = soEnemiesMover.distanceNext;
            _speedPerKillMult = soEnemiesMover.speedPerKillMult;

            _currentCooldown = _baseCooldown;
        }

        public void Initialize(List<Enemy> enemies, int totalEnemies)
        {
            StopRunning();

            _cts = new CancellationTokenSource();

            _enemies = enemies ?? new List<Enemy>();
            _totalEnemies = Mathf.Max(1, totalEnemies);
            _aliveCount = _enemies.Count;

            _isRightSide = true;
            _enemiesTransform.position = _startPos;
            _targetPos = _startPos;

            _currentCooldown = _baseCooldown;
            _allClearedInvoked = false;

            Run(_cts.Token).Forget();
        }

        private async UniTaskVoid Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UpdateAliveCount();

                if (_aliveCount <= 0)
                {
                    InvokeAllCleared();
                    break;
                }

                _currentCooldown = CalculateCooldown();

                bool isCanceled = await UniTask.WaitForSeconds(
                        _currentCooldown,
                        ignoreTimeScale: false,
                        cancellationToken: token)
                    .SuppressCancellationThrow();

                if (isCanceled) break;

                Move(_currentCooldown);

                if (_allClearedInvoked) break;
            }
        }

        private void UpdateAliveCount()
        {
            int count = 0;

            if (_enemies != null)
            {
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy != null && enemy.Alive)
                    {
                        count++;
                    }
                }
            }

            _aliveCount = count;
        }

        private float CalculateCooldown()
        {
            if (_totalEnemies <= 0 || _aliveCount <= 0)
            {
                return _baseCooldown;
            }

            float removedRatio = 1f - (float)_aliveCount / _totalEnemies;
            removedRatio = Mathf.Clamp01(removedRatio);

            float multiplier = 1f + (float)_speedPerKillMult * removedRatio;
            multiplier = Mathf.Max(0.01f, multiplier);

            return Mathf.Max(0.05f, _baseCooldown / multiplier);
        }

        private void Move(float duration)
        {
            _enemiesTransform.DOKill();

            if (!TryGetAliveBounds(out float minX, out float maxX))
            {
                InvokeAllCleared();
                return;
            }

            Vector3 target = _targetPos;

            if (_isRightSide)
            {
                if (_xMax - maxX <= _distanceNext)
                {
                    _isRightSide = false;
                    target.y -= _stepSize;
                }
                else
                {
                    target.x += _stepSize;
                }
            }
            else
            {
                if (minX - _xLow <= _distanceNext)
                {
                    _isRightSide = true;
                    target.y -= _stepSize;
                }
                else
                {
                    target.x -= _stepSize;
                }
            }

            _targetPos = target;
            _enemiesTransform.DOMove(target, duration);
        }

        private bool TryGetAliveBounds(out float minX, out float maxX)
        {
            minX = float.MaxValue;
            maxX = float.MinValue;

            bool any = false;

            if (_enemies != null)
            {
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy == null || !enemy.Alive) continue;

                    float x = enemy.GetPositionX();

                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;

                    any = true;
                }
            }

            return any;
        }

        private void InvokeAllCleared()
        {
            if (_allClearedInvoked) return;

            _allClearedInvoked = true;

            StopRunning();
            OnAllEnemiesCleared?.Invoke();
        }

        private void StopRunning()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _enemiesTransform?.DOKill();
        }

        public void OnGameClose()
        {
            StopRunning();
        }
    }
}