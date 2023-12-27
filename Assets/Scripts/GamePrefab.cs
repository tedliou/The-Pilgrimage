using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/GamePrefab", fileName = "GamePrefab")]
public class GamePrefab : ScriptableObject
{
    [SerializeField] private List<BlockBase> _prefab;

    private Dictionary<string, BlockBase> _prefabDict = new Dictionary<string, BlockBase>();
    
    public void Init()
    {
        foreach (var e in _prefab)
        {
            Debug.Log($"[{nameof(GameManager)}] 註冊 {e.Setting.Id} ({e})");
            _prefabDict.Add(e.Setting.Id, e);
        }
    }

    public BlockBase GetPrefab(string id)
    {
        if (_prefabDict.TryGetValue(id, out BlockBase result))
        {
            return result;
        }

        return null;
    }
}