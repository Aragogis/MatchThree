using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefaultObject : MonoBehaviour
{
    [SerializeField] public ObjType type;
    public GemList gemList;
    public Vector3 pos;

    public abstract HashSet<GameObject> GetDestrPattern();
    public virtual void UpdatePos() { }
    public virtual IEnumerator StartDestroyAnimation() { yield return null; }
    public  List<GameObject> GetNeighboursFlattened()
    {
        List<GameObject> neighbours = new List<GameObject>();
        neighbours.AddRange(this.GetHorizontalNeighbours());
        neighbours.AddRange(this.GetVerticalNeighbours());

        return neighbours;
    }
    public  GameObject[][] GetNeighbours()
    {
        GameObject[][] neighbours = new GameObject[2][];
        neighbours[0] = new GameObject[2];
        neighbours[1] = new GameObject[2];

        neighbours[0] = this.GetHorizontalNeighbours();
        neighbours[1] = this.GetVerticalNeighbours();

        return neighbours;
    }

    public GameObject[] GetHorizontalNeighbours()
    {
        GameObject[] neighbours = new GameObject[2];
        Vector3 gemPos = this.pos;

        if (gemPos.x - 1 >= 0)
            neighbours[0] = gemList[gemPos.x - 1, gemPos.y];
        else neighbours[0] = null;
        if (gemPos.x + 1 < gemList.colss)
            neighbours[1] = gemList[gemPos.x + 1, gemPos.y];
        else neighbours[1] = null;

        return neighbours;
    }

    public GameObject[] GetVerticalNeighbours()
    {
        GameObject[] neighbours = new GameObject[2];
        Vector3 gemPos = this.pos;

        if (gemPos.y + 1 < gemList.rowss)
            neighbours[0] = gemList[gemPos.x, gemPos.y + 1];
        else neighbours[0] = null;
        if (gemPos.y - 1 >= 0)
            neighbours[1] = gemList[gemPos.x, gemPos.y - 1];
        else neighbours[1] = null;

        return neighbours;
    }
    // Start is called before the first frame update
    void Awake()
    {
        this.gemList = FindObjectOfType<GemList>();
    }

    private void OnDestroy()
    {
        GameEvents.TriggerObjectDestroyed(type);
    }
}
