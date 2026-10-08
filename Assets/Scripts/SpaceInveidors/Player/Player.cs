using System.Threading;

namespace SpaceInvaders.Player
{
    using System;
    using Cysharp.Threading.Tasks;
    using SpaceInveidors;
    using SpaceInveidors.Bullet;
    using SpaceInveidors.Player.Component;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Player : MonoBehaviour, IDamage
    {
        public static Player Instance { get; private set; }

        [Header("Movement")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        [Header("Shooting")]
        [SerializeField] private float _shotCooldown = 0.2f;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _speedBullet = 10f;

        [Header("Life")]
        [SerializeField] private float _respawnDelay = 1f;
        [SerializeField] private float _invulnerabilityDuration = 2f;
        [SerializeField] private SpriteRenderer _spriteRender;
        [SerializeField] private Collider2D _collider;

        private Mover _mover;
        private Shoter _shoter;
        private InputSystem_Actions _actions;
        private IBulletProvider _bulletProvider;

        private Vector2 _vector2Input;
        private Vector3 _spawnPosition;

        private bool _isDead;
        private bool _isInvulnerable;

        public bool IsAlive => !_isDead;
        public bool CanBeDamaged => !_isDead && !_isInvulnerable;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _spawnPosition = transform.position;

            _actions = new InputSystem_Actions();

            _mover = new Mover(_rigidbody2D, _speed);

            _bulletProvider = BulletService.Instance;

            _shoter = new Shoter(
                _startPoint,
                _bulletProvider,
                _damage,
                _speedBullet,
                _shotCooldown
            );
            

        }

        private void OnEnable()
        {
            _actions.Enable();
            _actions.Player.Attack.performed += OnShootPerformed;
        }

        private void OnDisable()
        {
            _actions.Player.Attack.performed -= OnShootPerformed;
            _actions.Disable();
        }

        private void FixedUpdate()
        {
            if (!IsAlive) return;

            _vector2Input = _actions.Player.Move.ReadValue<Vector2>();
            _mover.Move(_vector2Input.x);
        }

        private void OnShootPerformed(InputAction.CallbackContext context)
        {
            if (!IsAlive) return;

            _shoter.Shoot();
        }

        public void ApplyDamage(int damage)
        {
            if (!CanBeDamaged) return;

            _isDead = true;

            SetVisible(false);
            SetColliderEnabled(false);
            ZeroVelocity();

            if (GameService.Instance == null)
            {
                Debug.LogError("Player: GameService is missing.", this);
                ReviveImmediate();
                return;
            }

            GameService.Instance.TakePlayerHit();

            if (GameService.Instance.Lives > 0)
            {
                RespawnAsync().Forget();
            }
        }

        private async UniTaskVoid RespawnAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();

            await UniTask.Delay(
                TimeSpan.FromSeconds(_respawnDelay),
                ignoreTimeScale: false,
                cancellationToken: token
            );

            if (token.IsCancellationRequested) return;

            if (GameService.Instance.Lives <= 0)
            {
                return;
            }

            transform.position = _spawnPosition;

            ZeroVelocity();
            SetColliderEnabled(true);

            _isDead = false;
            _isInvulnerable = true;

            SetVisible(true);

            await BlinkAsync(_invulnerabilityDuration, token);

            if (token.IsCancellationRequested) return;

            _isInvulnerable = false;
            SetVisible(true);
        }

        private async UniTask BlinkAsync(float duration, CancellationToken token)
        {
            const float interval = 0.1f;

            float elapsed = 0f;
            bool visible = true;

            while (elapsed < duration && !token.IsCancellationRequested)
            {
                visible = !visible;
                SetVisible(visible);

                await UniTask.Delay(
                    TimeSpan.FromSeconds(interval),
                    ignoreTimeScale: false,
                    cancellationToken: token
                );

                elapsed += interval;
            }

            if (!token.IsCancellationRequested)
            {
                SetVisible(true);
            }
        }

        private void ReviveImmediate()
        {
            _isDead = false;
            _isInvulnerable = false;

            SetVisible(true);
            SetColliderEnabled(true);
        }

        private void ZeroVelocity()
        { 
            _rigidbody2D.linearVelocity = Vector2.zero;
        }

        private void SetVisible(bool visible)
        {
            _spriteRender.enabled = visible;
        }

        private void SetColliderEnabled(bool enabled)
        {
            _collider.enabled = enabled;
        }
    }
}