using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GemManager : MonoBehaviour
{
    [SerializeField] Gem[] gemVariants;
    [SerializeField] Transform gemSpawnAnchor;

    public Gem[,] gemArray;
    private GameState state = GameState.None;
    private GameObject gemHit;
    private  Vector3 velocity = new Vector3(10f,10f);
    void Awake()
    {
        InitializeGameField();
    }

    private void InitializeGameField()
    {
        // find spawn positions for gems, generate gems, check for right gem arrangement
        gemArray = new Gem[FieldParams.rows, FieldParams.cols];
        for(int i = 0; i < FieldParams.rows; i++)
        {
            for(int j = 0; j < FieldParams.cols; j++)
            {
                Gem newGem = gemVariants[UnityEngine.Random.Range(0, gemVariants.Length)];
                float xPos = gemSpawnAnchor.position.x + j;
                float yPos = gemSpawnAnchor.position.y - i;
                gemArray[i, j] = Instantiate<Gem>(newGem, new Vector3(xPos,yPos), Quaternion.identity);
                gemArray[i, j].GetComponent<Gem>().rowPos = i;
                gemArray[i,j].GetComponent<Gem>().colPos = j;
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
                Debug.Log(hit.collider);
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
                Debug.Log(hit.collider);
                if(hit.collider != null & hit.collider.gameObject != gemHit)
                {
                    if(!Utilities.AreNeighbours(gemHit, hit.collider.gameObject))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        Vector3 startPos1 = gemHit.transform.position;
                        Vector3 startPos2 = hit.transform.position;
                        StartCoroutine(AnimateSwap(gemHit, hit.collider.gameObject, startPos1, startPos2));
                        state = GameState.None;
                        velocity = new Vector3(10f, 10f);
                    }
                }
            }
        }
    }
    private IEnumerator AnimateSwap(GameObject gemHit, GameObject gemHit2, Vector3 startPos1, Vector3 startPos2)
    {


        float timeElapsed = 0;


        while (timeElapsed < FieldParams.swapDuration)
        {
            gemHit.transform.position = Vector3.SmoothDamp(gemHit.transform.position, startPos2, ref velocity, FieldParams.swapDuration);
            gemHit2.transform.position = Vector3.SmoothDamp(gemHit2.transform.position, startPos1, ref velocity, FieldParams.swapDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        gemHit.transform.position = startPos2;
        gemHit2.transform.position = startPos1;
    }
    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit, GameObject gemHit)
    {
        
        yield return null;
    }
}
