using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private QuestData questData;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI questText;
    public Slider turnsSlider;
    public GameObject levelCompleted;
    public int turnsLeft;

    public int TurnsLeft
    {
        get => turnsLeft;
        set
        {
            turnsLeft = value;
            UpdateTurnsSlider();
        }
    }

    private void UpdateTurnsSlider()
    {
        turnsSlider.value = turnsLeft;
    }

    public void DecreaseTurns()
    {
        TurnsLeft -= 1;
        if (TurnsLeft <= 0) Debug.Log("Game Over");
    }

    public void InitializeTurnsSlider(int turnsData)
    {
        turnsSlider.maxValue = turnsData;
        TurnsLeft = turnsData;
    }
    public int CurrentScore 
    {
        get => currentScore; 
        set
        {
            currentScore = value;
            UpdateScoreText();
        }
        
    }

    public QuestData QuestData 
    { 
        get => questData; 
        set 
        {
            questData = value;
            UpdateQuestText();
        } 
    }
    private void OnEnable()
    {
        GameEvents.OnObjectDestroyed += CheckQuestProgress;
        UpdateScoreText();
    }

    public void UpdateScore(HashSet<GameObject> gems)
    {
        int scoreSum = 0;
        foreach (var gem in gems)
        {
            if(gem == null) continue;
            ObjType gemType = gem.GetComponent<DefaultObject>().type;
            scoreSum += FieldParams.gemsValue[gemType];
        }
        this.CurrentScore += scoreSum * gems.Count;
    }

    private void UpdateScoreText()
    {
        scoreText.text = this.CurrentScore.ToString() + "\n";
    }
    private void UpdateQuestText()
    {
        questText.text = "";

        foreach (var quest in this.QuestData.quests)
        {
            questText.text += quest.Key.ToString() + ": " + quest.Value.ToString() + "\n";
        }
    }

    private void CheckQuestProgress(ObjType objType)
    {
        if (questData.quests.ContainsKey(objType))
        {
            questData.DecreaseCount(objType);
            UpdateQuestText();
            if (questData.isFinished)
            {
                Debug.Log("Quest Completed!");
            }
        }
    }
}
