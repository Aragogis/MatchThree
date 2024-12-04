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
        gemList[(int)pos.x, (int)pos.y] = this.gameObject;
    }
    public abstract List<GameObject> GetDestrPattern();
    public List<GameObject> GetNeighbours()
    {
        List<GameObject> neighbours = new List<GameObject>();

        Vector3 gemPos = this.pos;
        if (gemPos.y + 1 < FieldParams.rows)
            neighbours.Add(gemList[(int)gemPos.x, (int)gemPos.y + 1]);
        else neighbours.Add(null);
        if (gemPos.y - 1 >= 0)
            neighbours.Add(gemList[(int)gemPos.x, (int)gemPos.y - 1]);
        else neighbours.Add(null);
        if (gemPos.x + 1 < FieldParams.cols)
            neighbours.Add(gemList[(int)gemPos.x + 1, (int)gemPos.y]);
        else neighbours.Add(null);
        if (gemPos.x - 1 >= 0)
            neighbours.Add(gemList[(int)gemPos.x - 1, (int)gemPos.y]);
        else neighbours.Add(null);

        return neighbours;
    }


}
