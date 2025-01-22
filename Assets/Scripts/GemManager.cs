using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;
using System;
public class GemManager : MonoBehaviour
{
    private GameObject gemSpawnAnchor;
    private ScoreManager scoreManager;
    public LevelData level;
    private GameState state = GameState.None;
    private GameObject gemHit;
    private GemList gemList;
    private List<GameObject> currentPair = new List<GameObject>();
    public Dictionary<ObjType, GameObject> gemObjects;
    internal GameObject GetNewGem()
    {
        return gemObjects[(ObjType)Random.Range(1, 6)];
    }
    private void LoadLevel(LevelData level)
    {
            gemList.InitializeGemList(level.rows, level.columns);
            ObjType[,] loadedGrid = new ObjType[level.columns, level.rows];
            for (int i = 0; i < level.columns; i++)
            {
                for (int j = 0; j < level.rows; j++)
                {
                    loadedGrid[i, j] = (ObjType)level.gems[i * level.rows + j];
                }
            }

            for (int i = 0; i < loadedGrid.GetLength(0); i++)
            {
                for (int j = 0; j < loadedGrid.GetLength(1); j++)
                {
                    if(gemObjects.TryGetValue(loadedGrid[i, j], out GameObject gem))
                    {
                        float xPos = gemSpawnAnchor.transform.position.x + i;
                        float yPos = gemSpawnAnchor.transform.position.y + j;
                        gemList[i, j] = Instantiate(gem, new Vector3(xPos, yPos), Quaternion.identity, gemSpawnAnchor.transform);
                        gemList[i, j].GetComponent<DefaultObject>().pos = new Vector3(i, j);
                    }
                    else
                    {
                        gemList[i, j] = null;
                    }

                }
            }
            QuestData questData = new QuestData();
            foreach (var quest in level.quests)
            {
                questData.AddQuest(quest.type, quest.count);
            }
        scoreManager.QuestData = questData;
        scoreManager.InitializeTurnsSlider(level.turns);
    }

    internal GameObject GetNewGem(GameObject prevGem)
    {
        GameObject gem = GetNewGem();
        while (Utilities.AreSameType(gem.GetComponent<DefaultObject>(), prevGem.GetComponent<DefaultObject>()))
        {
            gem = GetNewGem();
        }
        return gem;
    }


    void Start()
    {
        gemList = FindObjectOfType<GemList>();
        gemSpawnAnchor = GameObject.FindWithTag("GemAnchor");
        scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        LoadLevel(level);
        CenterGrid();
        StartCoroutine(MatchController());
    }


    void Update()
    {
        if (state == GameState.None)
        {
            //user has clicked or touched
            if (Input.GetMouseButtonDown(0))
            {
                //get the hit position
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && !Utilities.IsBlock(hit.collider.gameObject))
                {
                    state = GameState.SelectionStarted;
                    gemHit = hit.collider.gameObject;
                }

            }
        }
        else if (state == GameState.SelectionStarted)
        {
            //user dragged
            if (Input.GetMouseButton(0))
            {

                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && gemHit != null && hit.collider.gameObject != gemHit && !Utilities.IsBlock(hit.collider.gameObject))
                {
                    if (!Utilities.AreNeighbours(gemHit.GetComponent<DefaultObject>(), hit.collider.gameObject.GetComponent<DefaultObject>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {

                        currentPair.Add(gemHit);
                        currentPair.Add(hit.collider.gameObject);
                        scoreManager.DecreaseTurns();
                        StartCoroutine(MatchController());

                    }
                }
            }
        }
    }

    IEnumerator MatchController()
    {
        state = GameState.Animating;
        if (currentPair.Count == 2)
        {
            yield return StartCoroutine(AnimateSwap(currentPair));
        }

        var matches = gemList.FindMatches();


        if (matches.Count < 3 && currentPair.Count == 2)
        {
            yield return StartCoroutine(AnimateSwap(currentPair));
        }
        else
        {
            while (matches.Count > 0)
            {
                yield return StartCoroutine(DestroyGems(matches));
                yield return StartCoroutine(DropGems());
                matches.Clear();
                matches.AddRange(gemList.FindMatches());
            }
        }

        currentPair.Clear();
        state = GameState.None;
    }

    IEnumerator AnimateSwap(List<GameObject> currentPair)
    {
        Vector3 startPosFirst = currentPair[0].transform.position;
        Vector3 startPosSecond = currentPair[1].transform.position;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * FieldParams.swapDuration;

            currentPair[0].transform.position = Vector3.Lerp(startPosFirst, startPosSecond, t);
            currentPair[1].transform.position = Vector3.Lerp(startPosSecond, startPosFirst, t);

            yield return null;
        }

        currentPair[0].transform.position = startPosSecond;
        currentPair[1].transform.position = startPosFirst;

        currentPair[0].GetComponent<DefaultObject>().UpdatePos();
        currentPair[1].GetComponent<DefaultObject>().UpdatePos();
    }


    private IEnumerator DestroyGems(HashSet<GameObject> uniqueMatches)
    {
        var matchGroups = GroupMatches(uniqueMatches);

        foreach (var matchGroup in matchGroups)
        {
            if (matchGroup.Any(x => Utilities.IsBomb(x))) continue;

            if(Utilities.CheckPattern(matchGroup, out GameObject gemToConvert, out ObjType gemType))
            {
                GameObject bomb = Instantiate(gemObjects[gemType], gemToConvert.transform.position, Quaternion.identity, gemSpawnAnchor.transform);
                gemList[gemToConvert.GetComponent<DefaultObject>().pos.x, gemToConvert.GetComponent<DefaultObject>().pos.y] = bomb;
                uniqueMatches.Remove(gemToConvert);
                Destroy(gemToConvert);
                bomb.GetComponent<DefaultObject>().UpdatePos();
            }

        }
        HashSet<GameObject> gemsToDestroy = new HashSet<GameObject>();

        foreach (var gem in  uniqueMatches)
        {
            gemsToDestroy.AddRange(gem.GetComponent<DefaultObject>().GetDestrPattern());
        }
        gemsToDestroy.RemoveWhere(x => x == null);
        scoreManager.UpdateScore(gemsToDestroy);
        List<Coroutine> destroyTasks = new List<Coroutine>();
        foreach (var gem in gemsToDestroy)
        {
            if(Utilities.IsBlock(gem))
            {
                gem.GetComponent<DefaultBlock>().DecreaseDurability();
                continue;
            }
            foreach (var obj in gem.GetComponent<DefaultGem>().GetNeighboursFlattened())
            {
                if (Utilities.IsBlock(obj)) obj.GetComponent<DefaultBlock>().DecreaseDurability();
            }

            var tempPos = gem.GetComponent<DefaultObject>().pos;
            destroyTasks.Add(StartCoroutine(gem.GetComponent<DefaultObject>().StartDestroyAnimation()));
            gemList[tempPos.x, tempPos.y] = null;
        }

        foreach(var task in destroyTasks)
        {
            yield return task;
        }
    }
    private List<List<GameObject>> GroupMatches(HashSet<GameObject> matches)
    {
        List<List<GameObject>> groups = new List<List<GameObject>>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        foreach (var gem in matches)
        {
            if (Utilities.IsBomb(gem)) continue;
            if (!visited.Contains(gem))
            {
                List<GameObject> group = new List<GameObject>();
                Queue<GameObject> queue = new Queue<GameObject>();
                queue.Enqueue(gem);
                GameObject prevGem = null;
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    if (!visited.Contains(current))
                    {
                        visited.Add(current);
                        group.Add(current);

                        var neighbours = current.GetComponent<DefaultObject>().GetNeighboursFlattened();
                        foreach (var neighbour in neighbours)
                        {
                            if (neighbour != null && 
                                matches.Contains(neighbour) && 
                                !visited.Contains(neighbour) && 
                                (Utilities.AreSameType(neighbour, current) || 
                                 Utilities.IsBomb(neighbour) || 
                                 Utilities.AreSameType(neighbour, prevGem)))
                            {
                                queue.Enqueue(neighbour);
                            }
                        }
                    }
                    prevGem = gem;
                }

                if (group.Count > 0)
                {
                    groups.Add(group);
                }
            }
        }

        return groups;
    }

    IEnumerator DropGems()
    {
        CreateNewGems();
        List<Coroutine> dropTasks = new List<Coroutine>();
        // Iterate through all gems in the gemList
        while (gemList.AnyDroppableNullSpaces())
        {

            foreach (var gem in gemList)
            {
                if (gem != null && !Utilities.IsBlock(gem))
                {
                    // Check if the position below the gem is within bounds and null
                    if (gem.GetComponent<DefaultObject>().pos.y - 1 >= 0) // Check if we're not at the bottom row
                    {
                        // Check if the space below the gem is empty (null)
                        if (gemList[gem.GetComponent<DefaultObject>().pos.x, gem.GetComponent<DefaultObject>().pos.y - 1] == null)
                        {
                            // If the space is empty, start dropping the gem
                            dropTasks.Add(StartCoroutine(DropGem(gem)));
                        }
                    }
                }
            }
            foreach (var task in dropTasks)
            {
                yield return task;
            }
            dropTasks.Clear();
            CreateNewGems();
        }
    }

    private void CreateNewGems()
    {
        for (int column = 0; column < gemList.colss; column++)
        {
            if (gemList[column, gemList.rowss - 1] == null)
            {
                GameObject newGem = GetNewGem();
                gemList[column, gemList.rowss - 1] = Instantiate(newGem, new Vector3(gemSpawnAnchor.transform.position.x + column, gemSpawnAnchor.transform.position.y + gemList.rowss - 1), Quaternion.identity, gemSpawnAnchor.transform);
                gemList[column, gemList.rowss - 1].GetComponent<DefaultObject>().pos = new Vector3(column, gemList.rowss - 1);
            }
        }
    }

    IEnumerator DropGem(GameObject gem)
    {
        Vector3 startPos = gem.transform.position;
        Vector3 startListPos = gem.GetComponent<DefaultObject>().pos;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - 1);
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * FieldParams.swapDuration * 6;

            gem.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        gem.transform.position = endPos;
        gem.GetComponent<DefaultObject>().UpdatePos();
        gemList[startListPos.x, startListPos.y] = null;
    }

    void CenterGrid()
    {
        float gridWidth = (gemList.colss - 1);
        float gridHeight = (gemList.rowss - 1);

        Vector2 gridBottomLeft = (Vector2)gemSpawnAnchor.transform.position;

        Vector2 gridCenter = gridBottomLeft + new Vector2(gridWidth / 2, gridHeight / 2);

        Vector2 screenCenter = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        Vector2 offset = screenCenter - gridCenter;

        gemSpawnAnchor.transform.position = gridBottomLeft + offset;
    }
}


    