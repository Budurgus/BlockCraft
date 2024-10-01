using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Blocks/Sides Block", order = 51)]
public class BlockInfoSides : BlockInfo
{
    public Vector2Int RowAndColumnUp;
    public Vector2Int RowAndColumnDown;

    public override Vector2Int GetPixelOffset(Vector3Int normal)
    {
        if (normal == Vector3Int.up) return RowAndColumnUp;
        if (normal == Vector3Int.down) return RowAndColumnDown;

        return base.GetPixelOffset(normal);
    }
}
