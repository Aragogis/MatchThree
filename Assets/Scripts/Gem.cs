using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] public GemType type;
    public Vector3 pos;
    private GemList gemArray;
    // Start is called before the first frame update
    void Start()
    {
        this.gemArray = FindObjectOfType<GemList>();
    }
    public void UpdatePos()
    {
        this.pos = this.transform.position;
        gemArray[(int)pos.x, (int)pos.y] = this.gameObject;
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
