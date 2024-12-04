using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : DefaultGem
{
    public override List<GameObject> GetDestrPattern()
    {
        {
            List<GameObject> gemsToDestroy = new List<GameObject>
        {
            this.gameObject
        };
            for (int i = 1; i <= FieldParams.explodeRadius; i++)
            {
                if ((int)this.pos.x + i < FieldParams.cols)
                    gemsToDestroy.Add(gemList[(int)this.pos.x + i][(int)this.pos.y]);
                if ((int)this.pos.x - i >= 0)
                    gemsToDestroy.Add(gemList[(int)this.pos.x - i][(int)this.pos.y]);

                if ((int)this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[(int)this.pos.x][(int)this.pos.y + i]);
                if ((int)this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[(int)this.pos.x][(int)this.pos.y - i]);

                if ((int)this.pos.x + i < FieldParams.cols && (int)this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[(int)this.pos.x + i][(int)this.pos.y + i]);
                if ((int)this.pos.x - i >= 0 && (int)this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[(int)this.pos.x - i][(int)this.pos.y - i]);

                if ((int)this.pos.x - i >= 0 && (int)this.pos.y + i < FieldParams.rows)
                    gemsToDestroy.Add(gemList[(int)this.pos.x - i][(int)this.pos.y + i]);
                if ((int)this.pos.x + i < FieldParams.cols && (int)this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[(int)this.pos.x + i][(int)this.pos.y - i]);
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
