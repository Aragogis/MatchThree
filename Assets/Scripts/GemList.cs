using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class GemList : MonoBehaviour, IEnumerable<GameObject>
{
    private List<List<GameObject>> gemList;

    private void Awake()
    {
        this.gemList = new List<List<GameObject>>();
        for (int i = 0; i < FieldParams.cols; i++)
        {
            gemList.Add(new List<GameObject>(new GameObject[FieldParams.rows])); // Initialize each row with columns
        }
    }
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

    public int bombCount
    {
        get
        {
            int bombCount = 0;
            foreach (var gem in this)
            {
                if (gem != null && gem.GetComponent<DefaultGem>().type == GemType.Bomb) bombCount++;
            }
            return bombCount;
        }
    }
    public HashSet<GameObject> FindMatches()
    {
        HashSet<GameObject> matches = new HashSet<GameObject>();

        foreach(var gem in this)
        {
            var horizontalNeighbours = gem.GetComponent<DefaultGem>().GetNeighbours()[0];
            var verticalNeighbours = gem.GetComponent<DefaultGem>().GetNeighbours()[1];
            GemType gemType = gem.GetComponent<DefaultGem>().type;
            if (horizontalNeighbours[0] != null &&
                horizontalNeighbours[1] != null && 
                horizontalNeighbours[0].GetComponent<DefaultGem>().type == gemType && 
                horizontalNeighbours[1].GetComponent<DefaultGem>().type == gemType)
            {
                matches.Add(gem);
                matches.AddRange(horizontalNeighbours);
            }


            if (verticalNeighbours[0] != null &&
                verticalNeighbours[1] != null && 
                verticalNeighbours[0].GetComponent<DefaultGem>().type == gemType && 
                verticalNeighbours[1].GetComponent<DefaultGem>().type == gemType)
            {
                matches.Add(gem);
                matches.AddRange(verticalNeighbours);
            }
        }
        return matches;
    }

    public HashSet<GameObject> FindBombMatches()
    {
        HashSet<GameObject> bombs = new HashSet<GameObject>();
        HashSet<GameObject> bombMatches = new HashSet<GameObject>();

        foreach(var gem in this)
        {
            if(gem.GetComponent<DefaultGem>().type == GemType.Bomb) bombs.Add(gem);
        }

        foreach (var bomb in bombs)
        {
            var horizontalNeighbours = bomb.GetComponent<DefaultGem>().GetNeighbours()[0];
            var verticalNeighbours = bomb.GetComponent<DefaultGem>().GetNeighbours()[1];

            if (horizontalNeighbours[0] != null &&
                horizontalNeighbours[1] != null &&
                horizontalNeighbours[0].GetComponent<DefaultGem>().type == horizontalNeighbours[1].GetComponent<DefaultGem>().type)
            {
                if(horizontalNeighbours[0].GetComponent<DefaultGem>().type == GemType.Bomb)
                {
                    bombMatches.AddRange(this);
                    continue;
                }
                bombMatches.AddRange(bomb.GetComponent<Bomb>().GetDestrPattern());
                continue;
            }

            if (verticalNeighbours[0] != null &&
                verticalNeighbours[1] != null &&
                verticalNeighbours[0].GetComponent<DefaultGem>().type == verticalNeighbours[1].GetComponent<DefaultGem>().type)
            {
                if (verticalNeighbours[0].GetComponent<DefaultGem>().type == GemType.Bomb)
                {
                    bombMatches.AddRange(this);
                    continue;
                }
                bombMatches.AddRange(bomb.GetComponent<Bomb>().GetDestrPattern());
            }

        }

        return bombMatches;
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
            List<GameObject> neighbours = gem.GetComponent<DefaultGem>().GetNeighbours().SelectMany(x => x).ToList();
            foreach (GameObject neighbour in neighbours)
            {
                if (neighbour == null) continue;
                Vector3 gemPos = gem.GetComponent<DefaultGem>().pos;
                Vector3 neighbourPos = neighbour.GetComponent<DefaultGem>().pos;

                this[gemPos.x, gemPos.y] = neighbour;
                this[neighbourPos.x, neighbourPos.y] = gem;
                
                gem.GetComponent<DefaultGem>().pos = neighbourPos;
                neighbour.GetComponent<DefaultGem>().pos = gemPos;

                if (FindMatches().Count >= 3)
                {
                    this[gemPos.x, gemPos.y] = gem;
                    this[neighbourPos.x, neighbourPos.y] = neighbour;

                    gem.GetComponent<DefaultGem>().pos = gemPos;
                    neighbour.GetComponent<DefaultGem>().pos = neighbourPos;

                    return false;
                }

                this[gemPos.x, gemPos.y] = gem;
                this[neighbourPos.x, neighbourPos.y] = neighbour;

                gem.GetComponent<DefaultGem>().pos = gemPos;
                neighbour.GetComponent<DefaultGem>().pos = neighbourPos;
            }
        }
        return true;
    }
}
