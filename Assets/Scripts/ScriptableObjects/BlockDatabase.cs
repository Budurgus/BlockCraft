using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blocks Database", menuName = "Blocks/Blocks Database", order = 51)]
public class BlockDatabase : ScriptableObject
{
    [SerializeField] public BlockInfo[] Blocks;

    private Dictionary<BlockType, BlockInfo> blocksCached = new Dictionary<BlockType, BlockInfo>();

    private void Init()
    {
        blocksCached.Clear();

        foreach (var blockInfo in Blocks)
        {
            blocksCached.Add(blockInfo.Type, blockInfo);
        }
    }

    public BlockInfo GetInfo(BlockType type)
    {
        if (blocksCached.Count == 0) Init();

        if (blocksCached.TryGetValue(type, out var blockInfo))
        {
            return blockInfo;
        }

        return null;
    }


}
