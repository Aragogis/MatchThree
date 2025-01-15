using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class GemList : MonoBehaviour, IEnumerable<GameObject>
{
    private List<List<GameObject>> gemList;
    public int colss;
    public int rowss;

    public IEnumerator<GameObject> GetEnumerator()
    {
        // Iterate through each row
        foreach (var row in gemList)
        {
            // Iterate through each column in the current row
            foreach (var gem in row)
            {
                // Yield return each gem
                yield return gem;
            }
        }
    }


    public GameObject this[float row, float column]
    {
        get
        {
            try
            {
                return this.gemList[(int)row][(int)column];
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        set
        {
            gemList[(int)row][(int)column] = value;
        }
    }
    public List<GameObject> this[float row]
    {
        get
        {
            try
            {
                return this.gemList[(int)row];
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
    
    public void InitializeGemList(int rows, int cols)
    {
        this.gemList = new List<List<GameObject>>();
        for (int i = 0; i < cols; i++)
        {
            gemList.Add(new List<GameObject>(new GameObject[rows])); // Initialize each row with columns
        }

        rowss = rows;
        colss = cols;
    }

    public List<GameObject> GetRow(float rowIndex)
    {
        List<GameObject> rowGems = new List<GameObject>();
        for(int col = 0; col < colss; col++)
        {
            rowGems.Add(this[col, rowIndex]);
        }
        return rowGems;
    }

    public HashSet<GameObject> FindMatches()
    {
        HashSet<GameObject> matches = new HashSet<GameObject>();

        foreach(var gem in this)
        {
            var horizontalNeighbours = gem.GetComponent<DefaultObject>().GetHorizontalNeighbours();
            var verticalNeighbours = gem.GetComponent<DefaultObject>().GetVerticalNeighbours();

            if (horizontalNeighbours[0] != null &&
                horizontalNeighbours[1] != null && 
                Utilities.AreSameType(horizontalNeighbours[0], gem) && 
                Utilities.AreSameType(horizontalNeighbours[1], gem))
            {
                matches.Add(gem);
                matches.AddRange(horizontalNeighbours);
            }


            if (verticalNeighbours[0] != null &&
                verticalNeighbours[1] != null &&
                Utilities.AreSameType(verticalNeighbours[0], gem) &&
                Utilities.AreSameType(verticalNeighbours[1], gem))
            {
                matches.Add(gem);
                matches.AddRange(verticalNeighbours);
            }

            if (horizontalNeighbours[0] != null &&
                horizontalNeighbours[1] != null &&
                ((Utilities.AreSameType(horizontalNeighbours[0], gem) && Utilities.AreBomb(horizontalNeighbours[1])) || 
                 (Utilities.AreSameType(horizontalNeighbours[1], gem) && Utilities.AreBomb(horizontalNeighbours[0])) ))
            {
                matches.Add(gem);
                matches.AddRange(horizontalNeighbours);
            }

            if (verticalNeighbours[0] != null &&
                verticalNeighbours[1] != null &&
                ((Utilities.AreSameType(verticalNeighbours[0], gem) && Utilities.AreBomb(verticalNeighbours[1])) ||
                 (Utilities.AreSameType(verticalNeighbours[1], gem) && Utilities.AreBomb(verticalNeighbours[0])) ))
            {
                matches.Add(gem);
                matches.AddRange(verticalNeighbours);
            }

            if (horizontalNeighbours[0] != null &&
                horizontalNeighbours[1] != null &&
                Utilities.AreBomb(gem))
            {
                if (Utilities.AreSameType(horizontalNeighbours[0], horizontalNeighbours[1]))
                {
                    matches.Add(gem);
                    matches.AddRange(horizontalNeighbours);
                }
                
                if(Utilities.AreSameType(verticalNeighbours[0], verticalNeighbours[1]))
                {
                    matches.Add(gem);
                    matches.AddRange(verticalNeighbours);
                }
            }

        }
        return matches;
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }





    public bool CheckGameOver() // todo include special gems check
    {
        var gems = gemList.SelectMany(row => row)
                  .Where(gem => gem != null)
                  .ToList();
        foreach (GameObject gem in gems)
        {
            List<GameObject> neighbours = gem.GetComponent<DefaultObject>().GetNeighbours().SelectMany(x => x).ToList();
            foreach (GameObject neighbour in neighbours)
            {
                if (neighbour == null) continue;
                Vector3 gemPos = gem.GetComponent<DefaultObject>().pos;
                Vector3 neighbourPos = neighbour.GetComponent<DefaultObject>().pos;

                this[gemPos.x, gemPos.y] = neighbour;
                this[neighbourPos.x, neighbourPos.y] = gem;
                
                gem.GetComponent<DefaultObject>().pos = neighbourPos;
                neighbour.GetComponent<DefaultObject>().pos = gemPos;

                if (FindMatches().Count >= 3)
                {
                    this[gemPos.x, gemPos.y] = gem;
                    this[neighbourPos.x, neighbourPos.y] = neighbour;

                    gem.GetComponent<DefaultObject>().pos = gemPos;
                    neighbour.GetComponent<DefaultObject>().pos = neighbourPos;

                    return false;
                }

                this[gemPos.x, gemPos.y] = gem;
                this[neighbourPos.x, neighbourPos.y] = neighbour;

                gem.GetComponent<DefaultObject>().pos = gemPos;
                neighbour.GetComponent<DefaultObject>().pos = neighbourPos;
            }
        }
        return true;
    }
}
