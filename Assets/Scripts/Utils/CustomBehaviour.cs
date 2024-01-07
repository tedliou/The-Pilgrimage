using UnityEngine;

public class CustomBehaviour<T> : MonoBehaviour
{
    protected void Log(object message, LogType type = LogType.Log)
    {
        var msg = $"[{typeof(T).Name}] {message}";
        Debug.unityLogger.Log(type, msg);
        
    }
}