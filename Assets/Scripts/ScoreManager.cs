using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private QuestData questData;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI questText;
    public GameObject levelCompleted;

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
    }
    private void Start()
    {
        UpdateScoreText();
    }
    public void UpdateScore(HashSet<GameObject> gems)
    {
        int scoreSum = 0;
        foreach (var gem in gems)
        {
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
