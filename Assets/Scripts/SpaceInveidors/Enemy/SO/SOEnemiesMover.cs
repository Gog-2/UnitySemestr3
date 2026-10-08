namespace SpaceInveidors.Enemy
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemiesMoverCFG", menuName = "SpaceInvaders/EnemiesMoverCFG")]
    public class SOEnemiesMover : ScriptableObject
    {
        [Tooltip("На сколько единиц смещать группу врагов за один шаг.")]
        public float stepSize = 0.2f;

        [Tooltip("Базовая задержка между шагами.")]
        public float coldown = 0.8f;

        [Tooltip("Расстояние до стены, при котором враги начинают поворачивать вниз.")]
        public float distanceNext = 0.25f;

        [Tooltip("Модификатор ускорения при уничтожении врагов.")]
        public double speedPerKillMult = 0.35;
    }
}
