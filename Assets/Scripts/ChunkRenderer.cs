using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 16;
    public const int ChunkHeight = 128;
    public const float BlockScale = 1.0f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    public BlockDatabase Blocks;

    private Mesh chunkMesh;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    private void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

    }

    private void RegenerateMesh()
    {
        verticies.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
        RegenerateMesh();
    }

    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPosition);

        if (GetBlockAtPosition(blockPosition) == 0) return;

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRigthSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, Vector3Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector3Int.up);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector3Int.down);
        }
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPositions)
    {
        if (blockPositions.x >= 0 && blockPositions.x < ChunkWidth &&
            blockPositions.y >= 0 && blockPositions.y < ChunkHeight &&
            blockPositions.z >= 0 && blockPositions.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPositions.x, blockPositions.y, blockPositions.z];
        }

        else
        {
            if (blockPositions.y < 0 || blockPositions.y >= ChunkHeight) return BlockType.Air;

            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;
            if (blockPositions.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPositions.x += ChunkWidth;
            }
            else if (blockPositions.x >= ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPositions.x -= ChunkWidth;
            }


            if (blockPositions.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPositions.z += ChunkWidth;
            }
            else if (blockPositions.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPositions.z -= ChunkWidth;
            }

            if (ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPositions.x, blockPositions.y, blockPositions.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }

    private void GenerateRigthSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPositions)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPositions) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPositions) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void AddLastVerticiesSquare()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private Vector2 numTexture(Vector2Int rowAndColumn)
    {
        int row = rowAndColumn.x;
        int column = rowAndColumn.y;
        var TextSize = 2048;
        var BlockTextSize = 128;
        return new Vector2((float)(column * BlockTextSize) / TextSize,
                          ((float)(TextSize - BlockTextSize * (row + 1)) / TextSize));
    }


    private void AddUvs(BlockType blockType, Vector3Int normal)
    {
        Vector2 uv;

        BlockInfo info = Blocks.GetInfo(blockType);

        if (info != null)
        {
            uv = numTexture(info.GetPixelOffset(normal));
        }
        else
        {
            uv = numTexture(new Vector2Int(1, 14));
        }

        for (int i = 0; i < 4; i++)
        {
            uvs.Add(uv);
        }
    }

}
