using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RowBomb : Bomb
{
    public override HashSet<GameObject> GetDestrPattern()
    {

        HashSet<GameObject> gemsToDestroy = new HashSet<GameObject>
        {
            this.gameObject
        };
        gemsToDestroy.AddRange(gemList.GetRow(this.pos.y));
        return gemsToDestroy;
    }
}
