using System;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string uid;
    public string email;

    [SerializeField]
    public List<ScoreEntry> scores;

    [Serializable]
    public class ScoreEntry
    {
        public int levelId;
        public int score;
    }

    public Dictionary<int, int> ScoresToDictionary()
    {
        if(scores == null) return null;
        Dictionary<int, int> dict = new Dictionary<int, int>();
        foreach (var entry in scores)
        {
            dict[entry.levelId] = entry.score;
        }
        return dict;
    }

    public User(string uid, string email)
    {
        this.uid = uid;
        this.email = email;
        this.scores = new List<ScoreEntry>();
    }

    public void UpdateScore(int level, int score)
    {
        ScoreEntry existingEntry = scores.Find(entry => entry.levelId == level);

        if (existingEntry != null)
        {
            scores.Find(entry => entry.levelId == level).score = Mathf.Max(existingEntry.score, score);
            GameEvents.TriggerNewScore(level, Mathf.Max(existingEntry.score, score));
        }
        else
        {
            scores.Add(new ScoreEntry { levelId = level, score = score });
            GameEvents.TriggerNewScore(level, score);
        }

    }
}
