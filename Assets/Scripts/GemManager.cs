using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GemManager : MonoBehaviour
{
    [SerializeField] Transform gemSpawnAnchor;

    [SerializeField] public GameObject[] gemVariants;
    private GameState state = GameState.None;
    private GameObject gemHit;
    private GemList gemList;
    private List<GameObject> currentPair = new List<GameObject>();
    int currentScore = 0;

    internal GameObject GetNewGem()
    {
        return gemVariants[Random.Range(0, gemVariants.Length)];
    }


    internal GameObject GetNewGem(GameObject prevGem)
    {
        GameObject gem = GetNewGem();
        while (Utilities.AreSameType(gem.GetComponent<DefaultGem>(), prevGem.GetComponent<DefaultGem>()))
        {
            gem = GetNewGem();
        }
        return gem;
    }


    void Start()
    {
        gemList = FindObjectOfType<GemList>();
        InitializeGameField();
        StartCoroutine(MatchController());
    }


    private void InitializeGameField()
    {

        // find spawn positions for gems, generate gems, check for right gem arrangement
        for (int i = 0; i < FieldParams.cols; i++)
        {

            for (int j = 0; j < FieldParams.rows; j++)
            {
                GameObject newGem = GetNewGem();
                if (i - 1 > 0 && gemList[i - 1, j].GetComponent<DefaultGem>().type == newGem.GetComponent<DefaultGem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                if (j - 1 > 0 && gemList[i, j - 1].GetComponent<DefaultGem>().type == newGem.GetComponent<DefaultGem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                if (gemList.bombCount == FieldParams.maxBombs && newGem.GetComponent<DefaultGem>().type == GemType.Bomb)
                {
                    newGem = GetNewGem(newGem);
                }
                float xPos = gemSpawnAnchor.position.x + i;
                float yPos = gemSpawnAnchor.position.y + j;
                gemList[i, j] = Instantiate<GameObject>(newGem, new Vector3(xPos, yPos), Quaternion.identity, this.gameObject.transform) as GameObject;
                gemList[i, j].GetComponent<DefaultGem>().pos = new Vector3(i, j);

            }
        }



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
                if (hit.collider != null)
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
                if (hit.collider != null & gemHit != null & hit.collider.gameObject != gemHit)
                {
                    if (!Utilities.AreNeighbours(gemHit.gameObject.GetComponent<DefaultGem>(), hit.collider.gameObject.GetComponent<DefaultGem>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {

                        currentPair.Add(gemHit);
                        currentPair.Add(hit.collider.gameObject);

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
        matches.AddRange(gemList.FindBombMatches());
        if (matches.Count < 3)
        {
            if (currentPair.Count == 2)
            {
                yield return StartCoroutine(AnimateSwap(currentPair));
            }
        }
        else
        {
            while (matches.Count > 0)
            {
                DestroyGems(matches);
                yield return StartCoroutine(DropGems());
                matches.Clear();
                matches.AddRange(gemList.FindMatches());
                matches.AddRange(gemList.FindBombMatches());
            }
        }

        currentPair.Clear();
        state = GameState.None;
    }

    IEnumerator AnimateSwap(List<GameObject> currentPair)
    {
        Vector3 startPosFirst = currentPair[0].GetComponent<DefaultGem>().pos;
        Vector3 startPosSecond = currentPair[1].GetComponent<DefaultGem>().pos;

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

        currentPair[0].GetComponent<DefaultGem>().UpdatePos();
        currentPair[1].GetComponent<DefaultGem>().UpdatePos();
    }


    private void DestroyGems(HashSet<GameObject> uniqueMatches)
    {

        foreach (var gem in uniqueMatches)
        {

            var tempPos = gem.GetComponent<DefaultGem>().pos;
            //gem.GetComponent<SpriteRenderer>().color = Color.black;
            Destroy(gem.gameObject);
            gemList[tempPos.x, tempPos.y] = null;
        }
    }


    IEnumerator DropGems()
    {

        List<Coroutine> dropTasks = new List<Coroutine>();
        // Iterate through all gems in the gemList
        while (gemList.Any(gem => gem == null))
        {
            foreach (var gem in gemList)
            {
                if (gem != null)
                {
                    // Check if the position below the gem is within bounds and null
                    if (gem.GetComponent<DefaultGem>().pos.y - 1 >= 0) // Check if we're not at the bottom row
                    {
                        // Check if the space below the gem is empty (null)
                        if (gemList[gem.GetComponent<DefaultGem>().pos.x, gem.GetComponent<DefaultGem>().pos.y - 1] == null)
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
        for (int column = 0; column < FieldParams.cols; column++)
        {
            if (gemList[column, FieldParams.rows - 1] == null)
            {
                GameObject newGem = GetNewGem();
                if (gemList.bombCount == FieldParams.maxBombs && newGem.GetComponent<DefaultGem>().type == GemType.Bomb)
                {
                    newGem = GetNewGem(newGem);
                }
                gemList[column, FieldParams.rows - 1] = Instantiate<GameObject>(newGem, new Vector3(column, FieldParams.rows - 1), Quaternion.identity, this.gameObject.transform) as GameObject;
                gemList[column, FieldParams.rows - 1].GetComponent<DefaultGem>().pos = new Vector3(column, FieldParams.rows - 1);
            }
        }
    }

    IEnumerator DropGem(GameObject gem)
    {
        Vector3 startPos = gem.GetComponent<DefaultGem>().pos;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - 1);
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * FieldParams.swapDuration * 6;

            gem.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        gem.transform.position = endPos;
        gem.GetComponent<DefaultGem>().UpdatePos();
        gemList[startPos.x, startPos.y] = null;
    }

}


    