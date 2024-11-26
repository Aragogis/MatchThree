using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
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
                if (gem != null && gem.GetComponent<Gem>().type == GemType.Bomb) bombCount++;
            }
            return bombCount;
        }
    }
    public List<GameObject> FindMatches(Gem gem)
    {
        // Find consecutive matches in the current row
        var rowMatches = Enumerable.Range(0, this.gemList[(int)gem.pos.x].Count)
            .Where(col => this.gemList[(int)gem.pos.x][col].GetComponent<Gem>().type == gem.type) // Filter by gem type
            .Select((col, index) => new { col, index })
            .GroupBy(x => x.index - x.col) // Group by consecutive indexes
            .Where(g => g.Count() >= 3) // Only consider groups with 3 or more consecutive elements
            .SelectMany(g => g.Select(x => this.gemList[(int)gem.pos.x][x.col])) // Select the actual GameObjects
            .ToList();

        // Find consecutive matches in the current column
        var colMatches = Enumerable.Range(0, this.gemList.Count)
            .Where(row => this.gemList[row][(int)gem.pos.y].GetComponent<Gem>().type == gem.type) // Filter by gem type
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
            if(gem.GetComponent<Gem>().type == GemType.Bomb) bombs.Add(gem);
        }

        foreach(var gem in bombs)
        {
            var neighbours = this.GetNeighbours(gem.GetComponent<Gem>());

            if (neighbours[0] != null && neighbours[1] != null)
            {
                if(neighbours[0].GetComponent<Gem>().type == neighbours[1].GetComponent<Gem>().type)
                {
                    if (neighbours[0].GetComponent<Gem>().type == GemType.Bomb)
                    {
                        bombMatches.AddRange(this);
                    }
                    else
                    {
                        bombMatches.AddRange(this.FindExplodePattern(gem.GetComponent<Gem>()));
                    }
                }

            }
            if(neighbours[2] != null && neighbours[3] != null)
            {
                if (neighbours[2].GetComponent<Gem>().type == neighbours[3].GetComponent<Gem>().type)
                {
                    if (neighbours[2].GetComponent<Gem>().type == GemType.Bomb)
                    {
                        bombMatches.AddRange(this);
                    }
                    else
                    {
                        bombMatches.AddRange(this.FindExplodePattern(gem.GetComponent<Gem>()));
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
            matches.AddRange(FindMatches(gem.GetComponent<Gem>()));
        }
        return matches.Distinct().ToList();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private List<GameObject> FindExplodePattern(Gem gem)
    {
        List<GameObject> gemsToDestroy = new List<GameObject>
        {
            gem.gameObject
        };
        for(int i = 1; i <= FieldParams.explodeRadius; i++)
        {
            if((int)gem.pos.x + i < FieldParams.cols) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x + i][(int)gem.pos.y]);
            if((int)gem.pos.x - i >= 0) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x - i][(int)gem.pos.y]);

            if((int)gem.pos.y + i < FieldParams.rows) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x][(int)gem.pos.y + i]);
            if((int)gem.pos.y - i >= 0) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x][(int)gem.pos.y - i]);

            if((int)gem.pos.x + i < FieldParams.cols && (int)gem.pos.y + i < FieldParams.rows) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x + i][(int)gem.pos.y + i]);
            if((int)gem.pos.x - i >= 0 && (int)gem.pos.y - i >= 0) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x - i][(int)gem.pos.y - i]);

            if((int)gem.pos.x - i >= 0 && (int)gem.pos.y + i < FieldParams.rows) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x - i][(int)gem.pos.y + i]);
            if((int)gem.pos.x + i < FieldParams.cols && (int)gem.pos.y - i >= 0) 
                gemsToDestroy.Add(this.gemList[(int)gem.pos.x + i][(int)gem.pos.y - i]);
        }
        return gemsToDestroy;
    }

    public List<GameObject> GetNeighbours(Gem gem)
    {
        List<GameObject> neighbours = new List<GameObject>();

        Vector3 gemPos = gem.GetComponent<Gem>().pos;
        if (gemPos.y + 1 < FieldParams.rows)
            neighbours.Add(this[(int)gemPos.x, (int)gemPos.y + 1]);
        else neighbours.Add(null);
        if (gemPos.y - 1 >= 0) 
            neighbours.Add(this[(int)gemPos.x, (int)gemPos.y - 1]);
        else neighbours.Add(null);
        if (gemPos.x + 1 < FieldParams.cols)
            neighbours.Add(this[(int)gemPos.x + 1, (int)gemPos.y]);
        else neighbours.Add(null);
        if (gemPos.x - 1 >= 0)
            neighbours.Add(this[(int)gemPos.x - 1, (int)gemPos.y]);
        else neighbours.Add(null);

        return neighbours;
    }
}
