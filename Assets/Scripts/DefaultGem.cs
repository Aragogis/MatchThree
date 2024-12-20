using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultGem : MonoBehaviour
{
    [SerializeField] public GemType type;
    public GemList gemList;
    public Vector3 pos;
    void Awake()
    {
        this.gemList = FindObjectOfType<GemList>();
    }
    public void UpdatePos()
    {
        this.pos = this.transform.position;
        gemList[pos.x, pos.y] = this.gameObject;
    }
    public abstract HashSet<GameObject> GetDestrPattern();
    public GameObject[][] GetNeighbours()
    {
        GameObject[][] neighbours = new GameObject[2][];
        neighbours[0] = new GameObject[2];
        neighbours[1] = new GameObject[2];

        Vector3 gemPos = this.pos;
        if (gemPos.y + 1 < FieldParams.rows)
            neighbours[0][0] = gemList[gemPos.x, gemPos.y + 1];
        else neighbours[0][0] = null;
        if (gemPos.y - 1 >= 0)
            neighbours[0][1] = gemList[gemPos.x, gemPos.y - 1];
        else neighbours[0][1] = null;


        if (gemPos.x + 1 < FieldParams.cols)
            neighbours[1][0] = gemList[gemPos.x + 1, gemPos.y];
        else neighbours[1][0] = null;
        if (gemPos.x - 1 >= 0)
            neighbours[1][1] = gemList[gemPos.x - 1, gemPos.y];
        else neighbours[1][1] = null;

        return neighbours;
    }


}
