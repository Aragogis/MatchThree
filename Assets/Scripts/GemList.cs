using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject this[int row, int column]
    {
        get
        {
            try
            {
                return this.gemList[row][column];
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        set
        {
            gemList[row][column] = value;
        }
    }
    public List<GameObject> this[int row]
    {
        get
        {
            try
            {
                return this.gemList[row];
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
    public List<GameObject> FindMatches(DefaultGem gem)
    {
        // Find consecutive matches in the current row
        var rowMatches = Enumerable.Range(0, this.gemList[(int)gem.pos.x].Count)
            .Where(col => this.gemList[(int)gem.pos.x][col].GetComponent<DefaultGem>().type == gem.type) // Filter by gem type
            .Select((col, index) => new { col, index })
            .GroupBy(x => x.index - x.col) // Group by consecutive indexes
            .Where(g => g.Count() >= 3) // Only consider groups with 3 or more consecutive elements
            .SelectMany(g => g.Select(x => this.gemList[(int)gem.pos.x][x.col])) // Select the actual GameObjects
            .ToList();

        // Find consecutive matches in the current column
        var colMatches = Enumerable.Range(0, this.gemList.Count)
            .Where(row => this.gemList[row][(int)gem.pos.y].GetComponent<DefaultGem>().type == gem.type) // Filter by gem type
            .Select((row, index) => new { row, index })
            .GroupBy(x => x.index - x.row) // Group by consecutive indexes
            .Where(g => g.Count() >= 3) // Only consider groups with 3 or more consecutive elements
            .SelectMany(g => g.Select(x => this.gemList[x.row][(int)gem.pos.y])) // Select the actual GameObjects
            .ToList();

        // Combine row and column matches and return as a list of GameObjects
        return rowMatches.Concat(colMatches).ToList();
    }

    public List<GameObject> FindBombMatches()
    {
        List<GameObject> bombs = new List<GameObject>();
        List<GameObject> bombMatches = new List<GameObject>();

        foreach(var gem in this)
        {
            if(gem.GetComponent<DefaultGem>().type == GemType.Bomb) bombs.Add(gem);
        }

        foreach(var gem in bombs)
        {
            var neighbours = gem.GetComponent<DefaultGem>().GetNeighbours();

            if (neighbours[0] != null && neighbours[1] != null)
            {
                if(neighbours[0].GetComponent<DefaultGem>().type == neighbours[1].GetComponent<DefaultGem>().type)
                {
                    if (neighbours[0].GetComponent<DefaultGem>().type == GemType.Bomb)
                    {
                        bombMatches.AddRange(this);
                    }
                    else
                    {
                        bombMatches.AddRange(gem.GetComponent<Bomb>().GetDestrPattern());
                    }
                }

            }
            if(neighbours[2] != null && neighbours[3] != null)
            {
                if (neighbours[2].GetComponent<DefaultGem>().type == neighbours[3].GetComponent<DefaultGem>().type)
                {
                    if (neighbours[2].GetComponent<DefaultGem>().type == GemType.Bomb)
                    {
                        bombMatches.AddRange(this);
                    }
                    else
                    {
                        bombMatches.AddRange(gem.GetComponent<Bomb>().GetDestrPattern());
                    }
                }

            }

        }

        return bombMatches;
    }

    public List<GameObject> FindMatches()
    {
        List<GameObject> matches = new List<GameObject>();
        foreach(var gem in this)
        {
            matches.AddRange(FindMatches(gem.GetComponent<DefaultGem>()));
        }
        return matches.Distinct().ToList();
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
            List<GameObject> neighbours = gem.GetComponent<DefaultGem>().GetNeighbours();
            foreach (GameObject neighbour in neighbours)
            {
                if (neighbour == null) continue;
                Vector3 gemPos = gem.GetComponent<DefaultGem>().pos;
                Vector3 neighbourPos = neighbour.GetComponent<DefaultGem>().pos;

                this[(int)gemPos.x, (int)gemPos.y] = neighbour;
                this[(int)neighbourPos.x, (int)neighbourPos.y] = gem;
                
                gem.GetComponent<DefaultGem>().pos = neighbourPos;
                neighbour.GetComponent<DefaultGem>().pos = gemPos;

                if (FindMatches(neighbour.GetComponent<DefaultGem>()).Count >= 3 || FindMatches(gem.GetComponent<DefaultGem>()).Count >= 3)
                {
                    this[(int)gemPos.x, (int)gemPos.y] = gem;
                    this[(int)neighbourPos.x, (int)neighbourPos.y] = neighbour;

                    gem.GetComponent<DefaultGem>().pos = gemPos;
                    neighbour.GetComponent<DefaultGem>().pos = neighbourPos;

                    return false;
                }

                this[(int)gemPos.x, (int)gemPos.y] = gem;
                this[(int)neighbourPos.x, (int)neighbourPos.y] = neighbour;

                gem.GetComponent<DefaultGem>().pos = gemPos;
                neighbour.GetComponent<DefaultGem>().pos = neighbourPos;
            }
        }
        return true;
    }
}
