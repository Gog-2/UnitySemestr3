using UnityEngine;

namespace SpaceInveidors.Enemy
{
    [RequireComponent(typeof(Collider2D))]
    public class BaseEnemy : Enemy
    {
        protected override void OnDead()
        {
            Debug.Log("Dead enemy");
            base.OnDead();
        }
    }
}
