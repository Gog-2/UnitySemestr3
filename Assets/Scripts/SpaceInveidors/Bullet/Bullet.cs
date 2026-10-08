namespace SpaceInveidors.Bullet
{
    using UnityEngine;
    using UnityEngine.Pool;

    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private float _speed;
        [SerializeField] private Vector2 _direction;
        [SerializeField] private float _lifeTime = 3f;

        private bool _released;
        private int _damage;
        private IObjectPool<Bullet> _pool;
        private float _timer;

        public void SetPool(IObjectPool<Bullet> pool)
        {
            _pool = pool;
        }

        public void ReUse(Vector3 position, float speed, int damage)
        {
            _released = false;

            transform.position = position;
            
            _rigidbody2D.position = position;
            _rigidbody2D.linearVelocity = Vector2.zero;

            _speed = speed;
            _damage = damage;
            _timer = _lifeTime;
        }

        private void Update()
        {
            if (_released) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                Despawn();
            }
        }

        private void FixedUpdate()
        {
            if (_released) return;
            
            _rigidbody2D.linearVelocity = _direction * _speed;

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_released || other == null) return;
            
            if (other.TryGetComponent<Bullet>(out Bullet otherBullet))
            {
                otherBullet.Despawn();
                Despawn();
                return;
            }
            
            if (other.TryGetComponent<IDamage>(out IDamage damageable))
            {
                damageable.ApplyDamage(_damage);
                Despawn();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_released) return;

            Despawn();
        }

        public void Despawn()
        {
            if (_released) return;

            _released = true;

            _pool.Release(this);
        }
    }
}