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
}
