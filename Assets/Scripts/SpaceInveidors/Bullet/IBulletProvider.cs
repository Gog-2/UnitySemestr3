namespace SpaceInveidors.Bullet
{
    using UnityEngine;

    public interface IBulletProvider
    {
        void SpawnPlayerBullet(Vector3 position, int damage, float speed);
        void SpawnEnemyBullet(Vector3 position, int damage, float speed);
    }
}
