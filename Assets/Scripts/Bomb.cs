using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : DefaultGem
{
    public override HashSet<GameObject> GetDestrPattern()
    {
        {
            HashSet<GameObject> gemsToDestroy = new HashSet<GameObject>
        {
            this.gameObject
        };
            for (int i = 1; i <= FieldParams.explodeRadius; i++)
            {
                if (this.pos.x + i < FieldParams.cols)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y]);
                if (this.pos.x - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y]);

                if (this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[this.pos.x, this.pos.y + i]);
                if (this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x, this.pos.y - i]);

                if (this.pos.x + i < FieldParams.cols && this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y + i]);
                if (this.pos.x - i >= 0 && this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y - i]);

                if (this.pos.x - i >= 0 && this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y + i]);
                if (this.pos.x + i < FieldParams.cols && this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y - i]);
            }
            return gemsToDestroy;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
