using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [SerializeField] GameObject[] gemVariants;
    [SerializeField] Transform gemSpawnAnchor;
    [SerializeField] List<GameObject> serializedGemArray;

    private GameState state = GameState.None;
    private GameObject gemHit;
    private Vector3 velocity1 = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;

    GameObject[,] gemArray = new GameObject[FieldParams.rows, FieldParams.cols];

void Awake()
    {
        InitializeGameField();
        foreach (var gem in gemArray)
        {
            serializedGemArray.Add(gem);
        }
    }

    private void InitializeGameField()
    {
        // find spawn positions for gems, generate gems, check for right gem arrangement
        for(int i = 0; i < FieldParams.rows; i++)
        {
            for(int j = 0; j < FieldParams.cols; j++)
            {
                GameObject newGem = gemVariants[UnityEngine.Random.Range(0, gemVariants.Length)];
                float xPos = gemSpawnAnchor.position.x + j;
                float yPos = gemSpawnAnchor.position.y - i;
                gemArray[i, j] = Instantiate<GameObject>(newGem, new Vector3(xPos,yPos), Quaternion.identity);
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
                        Vector3 startPos1 = gemHit.transform.position;
                        Vector3 startPos2 = hit.transform.position;
                        StartCoroutine(AnimateSwap(gemHit, hit.collider.gameObject, startPos1, startPos2));
                        state = GameState.None;
                        velocity1 = Vector3.zero;
                        velocity2 = Vector3.zero;

                    }
                }
            }
        }
    }
    //private IEnumerator DestroyAndSpawnNewGem(GameObject gem)
    //{
    //    float timeElapsed = 0f;
    //    GameObject newGem = gemArray[(int)gem.GetComponent<Gem>().pos.x, (int)gem.GetComponent<Gem>().pos.y] = Instantiate<GameObject>(gemVariants[UnityEngine.Random.Range(0, gemVariants.Length)], gem.transform.position, Quaternion.identity);
    //    newGem.transform.localScale = Vector3.zero;
    //    newGem.GetComponent<Gem>().pos = gem.GetComponent<Gem>().pos;
    //    while(timeElapsed < FieldParams.swapDuration+1f)
    //    {
    //        newGem.transform.localScale = Vector3.Lerp(Vector3.zero, gem.transform.localScale,0.5f);
    //        timeElapsed += Time.deltaTime;

    //    }
    //    Destroy(gem);
    //    yield return null;
    //}
    private IEnumerator AnimateSwap(GameObject gemHit, GameObject gemHit2, Vector3 startPos1, Vector3 startPos2)
    {

        
        float timeElapsed = 0;

        while (timeElapsed < FieldParams.swapDuration+1f)
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
        gemHit2.GetComponent <Gem>().pos = tempPos;
        StartCoroutine(FindMatchesAndCollapse(gemHit));
    }
    private IEnumerator FindMatchesAndCollapse(GameObject hit)
    {
        Gem gem = hit.GetComponent<Gem>();
        List<GameObject> horizontalMatches = new List<GameObject>();
        List<GameObject> verticalMatches = new List<GameObject>();
        horizontalMatches.Add(hit);
        verticalMatches.Add(hit);
        for(int i = 1; gem.pos.x - i > 0 ; i++) 
        {
            if(gem.type == gemArray[((int)gem.pos.x - i), (int)gem.pos.y].GetComponent<Gem>().type)
            {
                horizontalMatches.Add(gemArray[(int)gem.pos.x - i, (int)gem.pos.y]);
            }
            else break;
        }
        for(int j = 1; gem.pos.x + j < FieldParams.rows; j++)
        {
            if (gem.type == gemArray[((int)gem.pos.x + j), (int)gem.pos.y].GetComponent<Gem>().type)
            {
                horizontalMatches.Add(gemArray[(int)gem.pos.x + j, (int)gem.pos.y]);
            }
            else break;
        }


        for(int k = 1; gem.pos.y - k > 0 ; k++)
        {
            if (gem.type == gemArray[(int)gem.pos.x, ((int)gem.pos.y - k)].GetComponent<Gem>().type)
            {
                verticalMatches.Add(gemArray[(int)gem.pos.x, ((int)gem.pos.y - k)]);
            }
            else break;
        }
        for (int l = 1; gem.pos.y + l < FieldParams.cols; l++)
        {
            if (gem.type == gemArray[(int)gem.pos.x, ((int)gem.pos.y + l)].GetComponent<Gem>().type)
            {
                verticalMatches.Add(gemArray[(int)gem.pos.x, ((int)gem.pos.y + l)]);
            }
            else break;
        }

        if (horizontalMatches.Count >= 3)
        {
            foreach (GameObject go in horizontalMatches)
            {
                gemArray[(int)go.GetComponent<Gem>().pos.x, (int)go.GetComponent<Gem>().pos.y] = null;
                Destroy(go);
            }
        }

        if(verticalMatches.Count >= 3)
        {
            foreach (GameObject go in verticalMatches)
            {
                gemArray[(int)go.GetComponent<Gem>().pos.x, (int)go.GetComponent<Gem>().pos.y] = null;
                Destroy(go);
            }
        }

        yield return null;
    }
}
