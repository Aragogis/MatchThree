using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : DefaultGem
{
    public override HashSet<GameObject> GetDestrPattern()
    {
        return new HashSet<GameObject>()
        {
            this.gameObject
        };
    }

    public override IEnumerator StartDestroyAnimation()
    {
        Animator gemAnimator = this.gameObject.GetComponent<Animator>();
        gemAnimator.Play("CollapseGem");
        yield return new WaitForSeconds(gemAnimator.GetCurrentAnimatorStateInfo(0).length - 0.5f);
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
