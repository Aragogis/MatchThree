using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject
{
    public int columns;
    public int rows;
    public int turns;
    public List<int> gems;
    public List<QuestEntry> quests;
}
