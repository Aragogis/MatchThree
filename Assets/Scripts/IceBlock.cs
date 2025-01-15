using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : DefaultBlock
{
    [SerializeField] public int durability = 3;

    public override void DecreaseDurability()
    {
        this.durability -= 1;
        if (this.durability == 0) Destroy(this.gameObject);
    }

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
