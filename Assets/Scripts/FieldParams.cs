using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GemType
{
    Red,
    Green,
    Blue
}
public enum GameState
{
    None,
    SelectionStarted,
    Animating
}
public class FieldParams
{
    public static int rows = 12;
    public static int cols = 8;
    public static float swapDuration = 1f;
}
