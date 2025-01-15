
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{

    internal static bool AreNeighbours(DefaultObject gemHit, DefaultObject gemHit2)
    {
        return ((gemHit.pos.x == gemHit2.pos.x) && ((gemHit.pos.y == gemHit2.pos.y + 1) || (gemHit.pos.y == gemHit2.pos.y - 1)))
            || (((gemHit.pos.y == gemHit2.pos.y) && ((gemHit.pos.x == gemHit2.pos.x + 1) || (gemHit.pos.x == gemHit2.pos.x - 1))));
    }

    internal static bool AreSameType(DefaultObject firstGem, DefaultObject secondGem)
    {
        return firstGem.type == secondGem.type;
    }

    internal static bool AreSameType(GameObject firstGem, GameObject secondGem)
    {
        if (firstGem == null || secondGem == null) return false;
        return firstGem.GetComponent<DefaultObject>().type == secondGem.GetComponent<DefaultObject>().type;
    }

    internal static bool AreBomb(GameObject gem)
    {
        if(gem == null) return false;
        ObjType gemType = gem.GetComponent<DefaultObject>().type;
        return (gemType == ObjType.Bomb || gemType == ObjType.RowBomb || gemType == ObjType.ColumnBomb);
    }

    internal static bool AreBomb(DefaultObject gem)
    {
        if (gem == null) return false;
        ObjType gemType = gem.type;
        return (gemType == ObjType.Bomb || gemType == ObjType.RowBomb || gemType == ObjType.ColumnBomb);
    }
    internal static bool AreBlock(GameObject obj)
    {
        if (obj == null) return false;
        ObjType objType = obj.GetComponent<DefaultObject>().type;
        return (objType == ObjType.IceBlock || objType == ObjType.UnbreakableBlock);
    }

    internal static bool AreBlock(DefaultObject obj)
    {
        if (obj == null) return false;
        ObjType objType = obj.type;
        return (objType == ObjType.IceBlock || objType == ObjType.UnbreakableBlock);
    }
    internal static bool IsCrossPattern(List<GameObject> gems, out GameObject gemToConvert)
    {
        int count = 0;
        gemToConvert = null;

        foreach (GameObject gem in gems)
        {
            var neighbours = gem.GetComponent<DefaultObject>().GetNeighboursFlattened();
            foreach(GameObject neighbour in neighbours)
            {
                if (gems.Contains(neighbour)) count++;
            }
            if (count == 4)
            {
                gemToConvert = gem;
            }
            else count = 0;
        }
        return count == 4;
    }

    internal static bool IsLongHorizontalPattern(List<GameObject> gems, out GameObject gemToConvert)
    {
        float yPos = gems[0].GetComponent<DefaultObject>().pos.y;
        gemToConvert = null;
        if(gems.Count <= 3) return false;

        foreach (var gem in gems)
        {
            if (gem.GetComponent<DefaultObject>().pos.y != yPos)
                return false;
        }

        gemToConvert = gems[gems.Count / 2];
        return true;
    }

    internal static bool IsLongVerticalPattern(List<GameObject> gems, out GameObject gemToConvert)
    {
        float xPos = gems[0].GetComponent<DefaultObject>().pos.x;
        gemToConvert = null;
        if (gems.Count <= 3) return false;

        foreach (var gem in gems)
        {
            if (gem.GetComponent<DefaultObject>().pos.x != xPos)
                return false;
        }

        gemToConvert = gems[gems.Count / 2];
        return true;
    }
}
