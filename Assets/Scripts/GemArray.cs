using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemArray
{
    private GameObject[,] gemArray;
    private Vector3 velocity1 = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;
    public GemArray(int row, int column)
    {
        this.gemArray = new GameObject[row, column];
    }
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return this.gemArray[row, column];
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        set
        {
            gemArray[row, column] = value;
        }
    }

    // Start is called before the first frame update
}
