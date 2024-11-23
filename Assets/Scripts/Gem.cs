using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] public GemType type;
    public Vector3 pos;
    private GemList gemList;
    // Start is called before the first frame update
    void Start()
    {
        this.gemList = FindObjectOfType<GemList>();
    }
    public void UpdatePos()
    {
        this.pos = this.transform.position;
        gemList[(int)pos.x, (int)pos.y] = this.gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //pos = this.transform.position;
    }
}
