using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GemObjects", menuName = "Game/Gem Objects")]
public class GemObjects : ScriptableObject
{
    [Serializable]
    public class GemEntry
    {
        public ObjType key;
        public GameObject value;
    }

    [SerializeField]
    private List<GemEntry> gemEntries = new List<GemEntry>();

    // Public method to access the Dictionary
    public Dictionary<ObjType, GameObject> ToDictionary()
    {
        Dictionary<ObjType, GameObject> dict = new Dictionary<ObjType, GameObject>();
        foreach (var entry in gemEntries)
        {
            dict[entry.key] = entry.value;
        }
        return dict;
    }
}
