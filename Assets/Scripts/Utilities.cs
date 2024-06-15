using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Utilities : MonoBehaviour
{
    internal static bool AreNeighbours(GameObject gemHit, GameObject gemHit2)
    {

        return (gemHit.GetComponent<Gem>().rowPos == gemHit2.GetComponent<Gem>().rowPos || gemHit.GetComponent<Gem>().colPos == gemHit2.GetComponent<Gem>().colPos);
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
