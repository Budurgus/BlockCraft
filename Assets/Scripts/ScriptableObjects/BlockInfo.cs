using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Blocks/Normal Block", order = 51)]
public class BlockInfo : ScriptableObject
{
    public BlockType Type;
    public Vector2Int RowAndColumn;

    public AudioClip StepSound;
    public float TimeToBreak;

    public virtual Vector2Int GetPixelOffset(Vector3Int normal)
    {
        return RowAndColumn;
    }
}
