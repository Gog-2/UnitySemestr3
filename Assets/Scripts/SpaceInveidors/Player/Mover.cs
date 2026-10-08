namespace SpaceInveidors.Player.Component
{
    using UnityEngine;

    public class Mover
    {
        private readonly Rigidbody2D _rigidbody2D;
        private readonly float _speed;

        public Mover(Rigidbody2D rigidbody2D, float speed)
        {
            _rigidbody2D = rigidbody2D;
            _speed = speed;
        }

        public void Move(float vector)
        {
            _rigidbody2D.linearVelocity = new Vector2(vector * _speed, 0);
        }
    }
}
