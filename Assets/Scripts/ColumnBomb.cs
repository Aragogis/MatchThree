using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColumnBomb : Bomb
{
    public override HashSet<GameObject> GetDestrPattern()
    {

        HashSet<GameObject> gemsToDestroy = new HashSet<GameObject>
        {
            this.gameObject
        };
        gemsToDestroy.AddRange(gemList[this.pos.x]);
        return gemsToDestroy;
    }

}
