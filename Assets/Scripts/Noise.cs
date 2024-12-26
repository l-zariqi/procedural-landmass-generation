using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    // Method for generating noise map, grid of values between 0-1
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth,mapHeight]; // 2D float array for noise map

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x; // Allows scrolling of the noise map
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue; // tracks the lowest and highest values
        float minNoiseHeight = float.MaxValue; 
        
        // When zooming into plane, zoom into centre instead of top right
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // Higher frequency makes points further apart
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfHeight) / scale * frequency - octaveOffsets[i].y * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Multiplying by 2 and subtracting by 1 to get negative values for more interesting noise
                    noiseHeight += perlinValue * amplitude;

                    // Amplitude decreases over time with persistance
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight; // Apply the noise height to noise map
            }
        }
        
        // Normalize values to be in the range of 0-1
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); // If noise map value = minNoiseHeight, return 0                                                                                 
            }
        }

                return noiseMap;
    }
}
