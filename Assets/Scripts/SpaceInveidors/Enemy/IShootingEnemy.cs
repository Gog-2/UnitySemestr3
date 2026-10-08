namespace SpaceInveidors.Enemy
{
    using SpaceInveidors.Bullet;

    public interface IShootingEnemy
    {
        void SetBulletProvider(IBulletProvider bulletProvider);
    }
}
