using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // To not overwrite the noise map with the colour map
    public enum DrawMode {NoiseMap, ColourMap, Mesh}; // Allows a selection of render modes
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;

    public float noiseScale;

    public int octaves;
    [Range(0,1)] // Makes persistance a slider in a range between 0-1
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier; // Allows the mesh terrain height to be altered
    public AnimationCurve meshHeightCurve; // Allows parts of the mesh terrain to be lowered

    public bool autoUpdate; // So that the noise map auto updates with new values

    public TerrainType[] regions; // Array of created regions

    public void GenerateMap() // Fetching 2D noise map from noise class
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight]; // 
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>(); // Takes the noise map and turns it into a texture, which is then applied to a plane
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
    }

    // Clamps values to be within a certain range
    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType // Allows the creation of different 
{
    public string name;
    public float height;
    public Color colour;
}
