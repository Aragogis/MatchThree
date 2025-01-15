using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void ObjectDestroyed(ObjType objType);
    public static event ObjectDestroyed OnObjectDestroyed;

    public static void TriggerObjectDestroyed(ObjType objType)
    {
        OnObjectDestroyed?.Invoke(objType);
    }
}
