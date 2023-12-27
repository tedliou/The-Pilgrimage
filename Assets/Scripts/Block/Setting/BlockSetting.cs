using UnityEngine;

public abstract class BlockSetting : ScriptableObject
{
    public string Id => name;
    public BlockType Type
    {
        get
        {
            return _type;
        }
    }
    [SerializeField] private BlockType _type;

    public abstract bool GetRequirement(out int id);
}

[System.Serializable]
public enum BlockType
{
    Building = 0,
    Floor = 1,
    Prop = 2,
    Chest = 3,
    Tool = 4,
    Float = 5
}