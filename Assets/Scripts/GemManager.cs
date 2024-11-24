using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GemManager : MonoBehaviour
{
    [SerializeField] Transform gemSpawnAnchor;

    [SerializeField] public GameObject[] gemVariants;
    private GameState state = GameState.None;
    private GameObject gemHit;
    private GemList gemList;
    private List<GameObject> currentPair = new List<GameObject>();


    internal GameObject GetNewGem()
    {
        return gemVariants[Random.Range(0, gemVariants.Length)];
    }


    internal GameObject GetNewGem(GameObject prevGem)
    {
        GameObject gem = GetNewGem();
        while (gem.GetComponent<Gem>().type == prevGem.GetComponent<Gem>().type)
        {
            gem = GetNewGem();
        }
        return gem;
    }


    void Start()
    {
        gemList = FindObjectOfType<GemList>();
        InitializeGameField();
    }


    private void InitializeGameField()
    {

        // find spawn positions for gems, generate gems, check for right gem arrangement
        for (int i = 0; i < FieldParams.cols; i++)
        {

            for (int j = 0; j < FieldParams.rows; j++)
            {
                GameObject newGem = GetNewGem();
                if (i - 1 > 0 && gemList[i - 1, j].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                if (j - 1 > 0 && gemList[i, j - 1].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                float xPos = gemSpawnAnchor.position.x + i;
                float yPos = gemSpawnAnchor.position.y + j;
                gemList[i, j] = Instantiate<GameObject>(newGem, new Vector3(xPos, yPos), Quaternion.identity) as GameObject;
                gemList[i, j].GetComponent<Gem>().pos.x = i;
                gemList[i, j].GetComponent<Gem>().pos.y = j;

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
                    if (!Utilities.AreNeighbours(gemHit.gameObject.GetComponent<Gem>(), hit.collider.gameObject.GetComponent<Gem>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        state = GameState.Animating;

                        currentPair.Add(gemHit);
                        currentPair.Add(hit.collider.gameObject);

                        StartCoroutine(DropGems(currentPair));

                        //check triples and >, return list of gems


                        //destroy matches
                        //spawn new gems
                        //count points


                    }
                }
            }
        }
    }


    IEnumerator AnimateSwap(List<GameObject> currentPair)
    {
        Vector3 startPosFirst = currentPair[0].GetComponent<Gem>().pos;
        Vector3 startPosSecond = currentPair[1].GetComponent<Gem>().pos;

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

        currentPair[0].GetComponent<Gem>().UpdatePos();
        currentPair[1].GetComponent<Gem>().UpdatePos();
    }


    IEnumerator FindMatches(List<GameObject> currentPair)
    {
        yield return StartCoroutine(AnimateSwap(currentPair));
        List<GameObject> matches;
        matches = gemList.FindMatches(currentPair[0].GetComponent<Gem>());
        matches.AddRange(gemList.FindMatches(currentPair[1].GetComponent<Gem>()));
        var uniqueMatches = matches.Distinct().ToList();

        if (uniqueMatches.Count < 3)
        {
            yield return StartCoroutine(AnimateSwap(currentPair));
            yield break;
        }
        DestroyGems(uniqueMatches);
    }


    private void DestroyGems(List<GameObject> uniqueMatches)
    {
        foreach (var gem in uniqueMatches)
        {
            var tempPos = gem.GetComponent<Gem>().pos;
            Destroy(gem.gameObject);
            gemList[(int)tempPos.x, (int)tempPos.y] = null;
        }
    }


    IEnumerator DropGems(List<GameObject> currentPair)
    {
        if(currentPair.Count == 2)
        {
            yield return StartCoroutine(FindMatches(currentPair));
        }
        List<GameObject> gemsToDrop = new List<GameObject>();
        // Iterate through all gems in the gemList
        while (gemList.Any(gem => gem == null))
        {
            foreach (var gem in gemList)
            {
                if (gem != null)
                {
                    // Check if the position below the gem is within bounds and null
                    if (gem.GetComponent<Gem>().pos.y - 1 >= 0) // Check if we're not at the bottom row
                    {
                        // Check if the space below the gem is empty (null)
                        if (gemList[(int)gem.GetComponent<Gem>().pos.x, (int)gem.GetComponent<Gem>().pos.y - 1] == null)
                        {
                            // If the space is empty, start dropping the gem
                            gemsToDrop.Add(gem);
                        }
                    }
                }
            }
            foreach (var gem in gemsToDrop)
            {
                Vector3 startPos = gem.GetComponent<Gem>().pos;
                Vector3 endPos = new Vector3(startPos.x, startPos.y - 1);
                float t = 0;

                while (t < 1f)
                {
                    t += Time.deltaTime * FieldParams.swapDuration*4;

                    gem.transform.position = Vector3.Lerp(startPos, endPos, t);

                    yield return null;
                }

                gem.transform.position = endPos;
                gem.GetComponent<Gem>().UpdatePos();
                gemList[(int)startPos.x, (int)startPos.y] = null;
            }

            gemsToDrop.Clear();
            for (int column = 0; column < FieldParams.cols; column++)
            {
                if (gemList[column, FieldParams.rows - 1] == null)
                {
                    GameObject newGem = GetNewGem();
                    gemList[column, FieldParams.rows - 1] = Instantiate<GameObject>(newGem, new Vector3(column, FieldParams.rows - 1), Quaternion.identity) as GameObject;
                    gemList[column, FieldParams.rows - 1].GetComponent<Gem>().pos.x = column;
                    gemList[column, FieldParams.rows - 1].GetComponent<Gem>().pos.y = FieldParams.rows - 1;
                }
            }
        }
        currentPair.Clear();
        List<GameObject> matchesAfterPair = new List<GameObject>();
        matchesAfterPair.AddRange(gemList.FindMatches());
        if (matchesAfterPair.Count > 0 )
        {
            DestroyGems(matchesAfterPair);
            matchesAfterPair.Clear();
            StartCoroutine(DropGems(currentPair));
        }
        else state = GameState.None;


    }

}


    