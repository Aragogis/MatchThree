using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : DefaultGem
{
    public override List<GameObject> GetDestrPattern()
    {
        return new List<GameObject>()
        {
            this.gameObject
        };
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
