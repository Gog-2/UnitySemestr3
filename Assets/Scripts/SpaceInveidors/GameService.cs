namespace SpaceInveidors
{
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [DefaultExecutionOrder(-100)]
    public class GameService : MonoBehaviour
    {
        public static GameService Instance { get; private set; }

        [SerializeField] private int _startingLives = 3;

        public int Lives { get; private set; }
        public int CurrentWave { get; private set; } = 1;

        public event Action<int> OnLivesChanged;
        public event Action OnGameOver;
        public event Action OnGameRestart;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Lives = _startingLives;
            CurrentWave = 1;

            OnLivesChanged?.Invoke(Lives);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }

        public void TakePlayerHit()
        {
            if (Lives <= 0) return;

            Lives--;
            OnLivesChanged?.Invoke(Lives);

            if (Lives <= 0)
            {
                OnGameOver?.Invoke();
            }
        }

        public void SetCurrentWave(int wave)
        {
            CurrentWave = Mathf.Max(1, wave);
        }

        public void RestartGame()
        {
            OnGameRestart?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
