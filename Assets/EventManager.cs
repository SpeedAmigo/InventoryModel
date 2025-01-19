using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<Transform> ItemIsHeld;
    public static event Action<Transform> ItemSnapped;
    
    public static void InvokeItemIsHeld(Transform item)
    {
        ItemIsHeld?.Invoke(item);
    }

    public static void InvokeItemSnapped(Transform item)
    {
        ItemSnapped?.Invoke(item);
    }
}
