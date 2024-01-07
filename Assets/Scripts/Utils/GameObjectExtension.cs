using UnityEngine;

public static class GameObjectExtension{
    public static void DestroySelf(this GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }
}