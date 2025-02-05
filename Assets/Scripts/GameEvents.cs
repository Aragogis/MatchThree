public static class GameEvents
{
    public delegate void ObjectDestroyed(ObjType objType);
    public static event ObjectDestroyed OnObjectDestroyed;

    public delegate void QuestCompleted();
    public static event QuestCompleted OnQuestCompleted;

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public delegate void NewScore(int levelId, int score);
    public static event NewScore OnNewScore;
    public static void TriggerObjectDestroyed(ObjType objType)
    {
        OnObjectDestroyed?.Invoke(objType);
    }

    public static void TriggerQuestCompleted() 
    {
        OnQuestCompleted?.Invoke();
    }

    public static void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }

    public static void TriggerNewScore(int levelId, int score)
    {
        OnNewScore?.Invoke(levelId, score);
    }
}
