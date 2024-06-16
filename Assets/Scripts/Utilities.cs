using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Utilities : MonoBehaviour
{
    internal static bool AreNeighbours(Gem gemHit, Gem gemHit2)
    {
        return ((gemHit.pos.x == gemHit2.pos.x) && ((gemHit.pos.y == gemHit2.pos.y + 1) || (gemHit.pos.y == gemHit2.pos.y - 1)))
            || (((gemHit.pos.y == gemHit2.pos.y) && ((gemHit.pos.x == gemHit2.pos.x + 1) || (gemHit.pos.x == gemHit2.pos.x - 1))));
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
