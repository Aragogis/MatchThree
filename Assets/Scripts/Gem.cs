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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
