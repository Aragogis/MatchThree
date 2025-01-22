
public abstract class DefaultGem : DefaultObject
{
    public override void UpdatePos()
    {
        this.pos.x = this.transform.localPosition.x;
        this.pos.y = this.transform.localPosition.y;
        gemList[pos.x, pos.y] = this.gameObject;
    }

    public void DestroyGem()
    {
        Destroy(this.gameObject);
    }
}
