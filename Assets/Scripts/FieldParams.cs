using System.Collections.Generic;
using System.Linq;

public enum ObjType
{
    None,
    Red = 1,
    Pink,
    Green,
    Blue,
    Yellow,
    Bomb,
    RowBomb,
    ColumnBomb,
    IceBlock,
    UnbreakableBlock
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}
public class FieldParams
{
    public static float swapDuration = 2.5f;
    public static int explodeRadius = 2;
    public static int maxBombs = 3;
    public static Dictionary<ObjType, int> gemsValue = new Dictionary<ObjType, int>()
    {
        { ObjType.Red, 10 },
        { ObjType.Green, 10 },
        { ObjType.Blue, 10 },
        { ObjType.Yellow, 10 },
        { ObjType.Pink, 10 },
        { ObjType.Bomb, 50 },
        { ObjType.RowBomb, 25 },
        { ObjType.ColumnBomb, 25 },
        { ObjType.IceBlock, 50 },
        { ObjType.UnbreakableBlock, 0}
    };
}


[System.Serializable]
public struct QuestEntry
{
    public ObjType type;
    public int count;
}
public class QuestData
{
    public Dictionary<ObjType, int> quests = new();
    public bool isFinished;
    public void AddQuest(ObjType type, int count)
    {
        quests.Add(type, count);
    }

    public void DecreaseCount(ObjType type)
    {
        if (quests.ContainsKey(type))
        {
            quests[type] -= 1;
            if (quests[type] < 0)
            {
                quests[type] = 0;
            }
        }
        if(quests.Values.All(count => count <= 0)) isFinished = true;
    }
}