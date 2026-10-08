using System;
using TMPro;
using UnityEngine;

public class ScoreService : MonoBehaviour
{
    [SerializeField] private string _nameOfScore;
    [SerializeField] private int _score = 0;
    [SerializeField] private TMP_Text _scoreText;

    public static ScoreService Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddScore(int score)
    {
        _score += score;
        UpdateUI();
    }

    private void UpdateUI()
    {
        _scoreText.text = _nameOfScore + _score.ToString();
    }
}
