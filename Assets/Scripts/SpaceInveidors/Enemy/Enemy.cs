namespace SpaceInveidors.Enemy
{
    using SpaceInveidors.Sound;
    using UnityEngine;

    public abstract class Enemy : MonoBehaviour, IDamage
    {
        [SerializeField] protected int _health = 1;
        [SerializeField] protected int _scorePerKill = 1;

        protected bool _alive = true;
        protected ISoundProvider _soundProvider;

        public bool Alive => _alive;
        public int Health => _health;
        public int ScorePerKill => _scorePerKill;

        public float GetPositionX()
        {
            return transform.position.x;
        }

        public virtual void SetSoundProvider(ISoundProvider soundProvider)
        {
            _soundProvider = soundProvider;
        }

        public void ApplyDamage(int damage)
        {
            if (!_alive) return;

            _health -= damage;

            if (_health <= 0)
            {
                _alive = false;
                OnDead();
            }
        }

        public virtual void Setup(int health, int scorePerKill)
        {
            _health = Mathf.Max(1, health);
            _scorePerKill = Mathf.Max(1, scorePerKill);
            _alive = true;
        }

        protected virtual void OnDead()
        {
            _soundProvider?.PlayEnemyDeath();

            ScoreService.Instance?.AddScore(_scorePerKill);
            gameObject.SetActive(false);
        }
    }
}
