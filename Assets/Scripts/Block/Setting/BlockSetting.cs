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