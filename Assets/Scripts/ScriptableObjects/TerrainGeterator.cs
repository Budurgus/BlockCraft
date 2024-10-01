using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain", menuName = "Terrain Generator", order = 51)]
public class TerrainGenerator: ScriptableObject
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;

    private FastNoiseLite warpNoise;

    public void Init()
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < octaveNoises.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        warpNoise = new FastNoiseLite();
        warpNoise.SetNoiseType(DomainWarp.NoiseType);
        warpNoise.SetFrequency(DomainWarp.Frequency);
        warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTarrain(int xOffset, int zOffset)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = GetHeight(x * ChunkRenderer.BlockScale + xOffset, z * ChunkRenderer.BlockScale + zOffset);
                float grassLayerHeight = 3;
                float dirtLayerHeight = 1;

                for (int y = 0; y < height / ChunkRenderer.BlockScale ; y++)
                {
                    if (height - y*ChunkRenderer.BlockScale < grassLayerHeight)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                    else if (height - y * ChunkRenderer.BlockScale < dirtLayerHeight) 
                    {
                        result[x, y, z] = BlockType.Dirt;
                    }
                    else 
                    {
                        result[x, y, z] = BlockType.Stone;
                    }
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        warpNoise.DomainWarp(ref x, ref y);

        float result = BaseHeight;

        for (int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}