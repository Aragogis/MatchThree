using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GemManager : MonoBehaviour
{
    [SerializeField] Transform gemSpawnAnchor;

    [SerializeField] public GameObject[] gemVariants;
    private GameState state = GameState.None;
    private GameObject gemHit;
    private GemArray gemArray = new GemArray(FieldParams.rows, FieldParams.cols);
    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();
    private Vector3 velocity1;
    private Vector3 velocity2;


    internal GameObject GetNewGem()
    {
        return gemVariants[Random.Range(0, gemVariants.Length)];
    }

    internal GameObject GetNewGem(GameObject prevGem)
    {
        GameObject gem = gemVariants[Random.Range(0, gemVariants.Length)];
        while (gem.GetComponent<Gem>().type == prevGem.GetComponent<Gem>().type)
        {
            gem = GetNewGem();
        }
        return gem;
    }

    void Awake()
    {
        InitializeGameField();
        StartCoroutine(CoroutineCoordinator());
    }

    private void InitializeGameField()
    {

        // find spawn positions for gems, generate gems, check for right gem arrangement
        for (int i = 0; i < FieldParams.rows; i++)
        {

            for(int j = 0; j < FieldParams.cols; j++)
            {
                GameObject newGem = GetNewGem();
                if (i - 1 > 0 && gemArray[i-1,j].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                if (j - 1 > 0 && gemArray[i, j-1].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type)
                {
                    newGem = GetNewGem(newGem);
                }
                float xPos = gemSpawnAnchor.position.x + j;
                float yPos = gemSpawnAnchor.position.y - i;
                gemArray[i, j] = Instantiate<GameObject>(newGem, new Vector3(xPos,yPos), Quaternion.identity) as GameObject;
                gemArray[i, j].GetComponent<Gem>().pos.x = i;
                gemArray[i, j].GetComponent<Gem>().pos.y = j;
                
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
                if(hit.collider != null & hit.collider.gameObject != gemHit)
                {
                    if(!Utilities.AreNeighbours(gemHit.gameObject.GetComponent<Gem>(), hit.collider.gameObject.GetComponent<Gem>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        state = GameState.Animating;
                        Vector3 startPos1 = gemHit.transform.position;
                        Vector3 startPos2 = hit.transform.position;
                        coroutineQueue.Enqueue(AnimateSwap(gemHit, hit.collider.gameObject, startPos1, startPos2));
                        coroutineQueue.Enqueue(FindMatchesAndCollapse(gemHit));
                        coroutineQueue.Enqueue(FindMatchesAndCollapse(hit.collider.gameObject));
                        state = GameState.None;
                        velocity1 = Vector3.zero;
                        velocity2 = Vector3.zero;

                    }
                }
            }
        }
    }

    internal IEnumerator FindMatchesAndCollapse(GameObject hit)
    {
        List<GameObject> verticalMatches = new List<GameObject>();
        List<GameObject> horizontalMatches = new List<GameObject>();
        Gem gem = hit.GetComponent<Gem>();
        verticalMatches.Add(hit);
        horizontalMatches.Add(hit);
        for (int i = 1; gem.pos.x - i >= 0; i++)
        {
            if (gemArray[((int)gem.pos.x - i), (int)gem.pos.y] != null && gem.type == gemArray[((int)gem.pos.x - i), (int)gem.pos.y].GetComponent<Gem>().type)
            {
                verticalMatches.Add(gemArray[(int)gem.pos.x - i, (int)gem.pos.y]);
            }
            else break;
        }
        for (int j = 1; gem.pos.x + j < FieldParams.rows; j++)
        {
            if (gemArray[((int)gem.pos.x + j), (int)gem.pos.y] != null && gem.type == gemArray[((int)gem.pos.x + j), (int)gem.pos.y].GetComponent<Gem>().type)
            {
                verticalMatches.Add(gemArray[(int)gem.pos.x + j, (int)gem.pos.y]);
            }
            else break;
        }


        for (int k = 1; gem.pos.y - k >= 0; k++)
        {
            if (gemArray[(int)gem.pos.x, ((int)gem.pos.y - k)] != null && gem.type == gemArray[(int)gem.pos.x, ((int)gem.pos.y - k)].GetComponent<Gem>().type)
            {
                horizontalMatches.Add(gemArray[(int)gem.pos.x, ((int)gem.pos.y - k)]);
            }
            else break;
        }
        for (int l = 1; gem.pos.y + l < FieldParams.cols; l++)
        {
            if (gemArray[(int)gem.pos.x, ((int)gem.pos.y + l)] != null && gem.type == gemArray[(int)gem.pos.x, ((int)gem.pos.y + l)].GetComponent<Gem>().type)
            {
                horizontalMatches.Add(gemArray[(int)gem.pos.x, ((int)gem.pos.y + l)]);
            }
            else break;
        }

        verticalMatches = verticalMatches.Distinct().ToList();
        horizontalMatches = horizontalMatches.Distinct().ToList();
        if (verticalMatches.Count >= 3)
        {
            foreach (GameObject go in verticalMatches)
            {
                //gemArray[(int)go.GetComponent<Gem>().pos.x, (int)go.GetComponent<Gem>().pos.y] = null;
                DestroyAndSpawnNewGem(go);
            }
        }

        if (horizontalMatches.Count >= 3)
        {
            foreach (GameObject go in horizontalMatches)
            {
                // gemArray[(int)go.GetComponent<Gem>().pos.x, (int)go.GetComponent<Gem>().pos.y] = null;
                DestroyAndSpawnNewGem(go);
            }
        }
        yield return null;
    }
    public IEnumerator AnimateSwap(GameObject gemHit, GameObject gemHit2, Vector3 startPos1, Vector3 startPos2)
    {


        float timeElapsed = 0;

        while (timeElapsed < FieldParams.swapDuration + 1f)
        {
            gemHit.transform.position = Vector3.SmoothDamp(gemHit.transform.position, startPos2, ref velocity1, FieldParams.swapDuration);
            gemHit2.transform.position = Vector3.SmoothDamp(gemHit2.transform.position, startPos1, ref velocity2, FieldParams.swapDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        gemHit.transform.position = startPos2;
        gemHit2.transform.position = startPos1;
        Vector2 tempPos = gemHit.GetComponent<Gem>().pos;
        gemHit.GetComponent<Gem>().pos = gemHit2.GetComponent<Gem>().pos;
        gemHit2.GetComponent<Gem>().pos = tempPos;
    }


    internal void DestroyAndSpawnNewGem(GameObject gem)
    {
        var pos = gem.GetComponent<Gem>().pos;
        Destroy(gem);

        GameObject newGem = Instantiate<GameObject>(GetNewGem(gem), gem.transform.position, Quaternion.identity) as GameObject;
        //if((gemArray[(int)gem.GetComponent<Gem>().pos.x + 1, (int)gem.GetComponent<Gem>().pos.y].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type &&
        //   gemArray[(int)gem.GetComponent<Gem>().pos.x - 1, (int)gem.GetComponent<Gem>().pos.y].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type)||
        //   (gemArray[(int)gem.GetComponent<Gem>().pos.x, (int)gem.GetComponent<Gem>().pos.y + 1].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type &&
        //   gemArray[(int)gem.GetComponent<Gem>().pos.x, (int)gem.GetComponent<Gem>().pos.y - 1].GetComponent<Gem>().type == newGem.GetComponent<Gem>().type))
        //{
        //    newGem = GetNewGem(newGem);
        //}
        newGem.GetComponent<Gem>().pos = pos;
        gemArray[(int)newGem.GetComponent<Gem>().pos.x, (int)newGem.GetComponent<Gem>().pos.y] = newGem;

    }

    private IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (coroutineQueue.Count > 0)
                yield return StartCoroutine(coroutineQueue.Dequeue());
            yield return null;
        }
    }
}
