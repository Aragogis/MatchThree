public enum GemType
{
    Red,
    Pink,
    Green,
    Blue,
    Yellow,
    Bomb
}
public enum GameState
{
    None,
    SelectionStarted,
    Animating
}
public class FieldParams
{
    public static int cols = 8;
    public static int rows = 12;
    public static float swapDuration = 2.5f;
    public static int explodeRadius = 2;
    public static int maxBombs = 3;
}
