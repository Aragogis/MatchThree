
public class Utilities
{

    internal static bool AreNeighbours(DefaultGem gemHit, DefaultGem gemHit2)
    {
        return ((gemHit.pos.x == gemHit2.pos.x) && ((gemHit.pos.y == gemHit2.pos.y + 1) || (gemHit.pos.y == gemHit2.pos.y - 1)))
            || (((gemHit.pos.y == gemHit2.pos.y) && ((gemHit.pos.x == gemHit2.pos.x + 1) || (gemHit.pos.x == gemHit2.pos.x - 1))));
    }

    internal static bool AreSameType(DefaultGem firstGem, DefaultGem secondGem)
    {
        return firstGem.type == secondGem.type;
    }

}
