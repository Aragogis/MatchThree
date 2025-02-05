using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GemObjects gemObjects;
    [SerializeField] private GemManager gemManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private FirebaseManager firebaseManager;

    [SerializeField] private GameObject scoreQuestPanel;
    [SerializeField] private GameObject turnsSlider;

    [SerializeField] private GameObject levelCompleted;
    [SerializeField] private TextMeshProUGUI levelCompletedScore;
    [SerializeField] private GameObject gameOver;

    [SerializeField] private List<TextMeshProUGUI> levelScores;
    private void OnEnable()
    {
        GameEvents.OnQuestCompleted += ShowLevelCompleted;
        GameEvents.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestCompleted -= ShowLevelCompleted;
        GameEvents.OnGameOver -= ShowGameOver;
    }
    public void StartLevel(LevelData level)
    {
        scoreManager.CurrentScore = 0;
        SwitchGameUI(true);
        gemManager.level = level;
        gemManager.gemObjects = gemObjects.ToDictionary();
        gemManager.StartLevel();
    }

    private void ShowLevelCompleted()
    {
        UpdateLevelScores();
        SwitchGameUI(false);
        levelCompletedScore.text = scoreManager.CurrentScore.ToString();
        levelCompleted.SetActive(true);
    }

    private void ShowGameOver()
    {
        SwitchGameUI(false);
        gameOver.SetActive(true);
    }

    private void SwitchGameUI(bool state)
    {
        scoreQuestPanel.SetActive(state);
        turnsSlider.SetActive(state);
    }

    public void UpdateLevelScores()
    {
        if(firebaseManager.user.ScoresToDictionary() != null)
        {
            Dictionary<int, int> userLevelScores = firebaseManager.user.ScoresToDictionary();
            for (int level = 0; level < levelScores.Count; level++)
            {
                if (userLevelScores.ContainsKey(level)) levelScores[level].text = userLevelScores[level].ToString();
            }
        }
    }
}
