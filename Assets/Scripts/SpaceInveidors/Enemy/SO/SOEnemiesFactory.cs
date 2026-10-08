namespace SpaceInveidors.Enemy
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemiesFactoryCFG", menuName = "SpaceInvaders/EnemiesFactoryCFG")]
    public class SOEnemiesFactory : ScriptableObject
    {
        public Enemy[] PrefabsEnemies;

        [Header("Base Grid")]
        public int VerticalAmmout = 4;
        public int HorizontalAmmout = 8;

        public float XOffSet = 0.6f;
        public float YOffSet = 0.5f;

        [Header("Wave Scaling")]
        [Tooltip("Сколько добавлять рядов каждую волну.")]
        public int VerticalPerWave = 0;

        [Tooltip("Сколько добавлять колонок каждую волну.")]
        public int HorizontalPerWave = 1;

        [Tooltip("Сколько добавлять HP каждому врагу каждую волну.")]
        public int HealthPerWave = 0;

        [Tooltip("Сколько добавлять очков за убийство каждую волну.")]
        public int ScorePerWave = 0;

        [Header("Limits")]
        public int MaxVerticalAmount = 8;
        public int MaxHorizontalAmount = 12;
    }
}
