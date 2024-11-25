using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] public GemType type;
    private GemList gemList;
    public Vector3 pos;

    void Start()
    {
        this.gemList = FindObjectOfType<GemList>();
    }
    public void UpdatePos()
    {
        this.pos = this.transform.position;
        gemList[(int)pos.x, (int)pos.y] = this.gameObject;
    }

    void Update()
    {
        
    }
}
