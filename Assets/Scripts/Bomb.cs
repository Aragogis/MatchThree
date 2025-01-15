using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : DefaultGem
{
    public override HashSet<GameObject> GetDestrPattern()
    {
        HashSet<GameObject> gemsToDestroy = new HashSet<GameObject>
        {
            this.gameObject
        };
        for (int i = 1; i <= FieldParams.explodeRadius; i++)
        {
            if (this.pos.x + i < gemList.colss)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y]);
            if (this.pos.x - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y]);

            if (this.pos.y + i < gemList.rowss)
                    gemsToDestroy.Add(gemList[this.pos.x, this.pos.y + i]);
            if (this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x, this.pos.y - i]);

            if (this.pos.x + i < gemList.colss && this.pos.y + i < gemList.rowss)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y + i]);
            if (this.pos.x - i >= 0 && this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y - i]);

            if (this.pos.x - i >= 0 && this.pos.y + i < gemList.rowss)
                    gemsToDestroy.Add(gemList[this.pos.x - i, this.pos.y + i]);
            if (this.pos.x + i < gemList.colss && this.pos.y - i >= 0)
                    gemsToDestroy.Add(gemList[this.pos.x + i, this.pos.y - i]);
        }
        return gemsToDestroy;
    }

    public override IEnumerator StartDestroyAnimation()
    {
        Animator gemAnimator = this.gameObject.GetComponent<Animator>();
        gemAnimator.Play("ExplodeBomb");
        yield return new WaitForSeconds(gemAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void Update()
    {

    }
}
