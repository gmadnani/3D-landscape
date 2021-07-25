using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum GenerateType { HeightMap, ColoredHeightMap, TerrainMesh }
    public GenerateType generateType;
    public int mapSize;

    public int randomRange;
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    [Range(0, 1)]
    public float scale;
    public int seed;
    public bool autoUpdate;
    public TerrainType[] regions;

    void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        float[,] heightMap = DiamondSquare.GenerateHeightMap(mapSize, seed, scale, randomRange);

        // Generate an array of colors for the vertices. Which colour
        // depends on which region the height corresponds to
        Color[] colors = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                foreach (TerrainType region in regions)
                {
                    if (heightMap[x, y] <= region.height)
                    {
                        colors[y * mapSize + x] = region.color;
                        break;
                    }
                }
            }
        }

        Texture2D terrainTexture = new Texture2D(mapSize, mapSize);
        // Found on a tutorial to make the texture more blocky and clamp the texture
        terrainTexture.filterMode = FilterMode.Point;
        terrainTexture.wrapMode = TextureWrapMode.Clamp;

        terrainTexture.SetPixels(colors);
        terrainTexture.Apply();

        Mesh terrainMesh = TerrainMeshScript.CreateTerrainMesh(heightMap, heightMultiplier, mapSize, heightCurve);

        TerrainDisplay display = FindObjectOfType<TerrainDisplay>();
        if (generateType == GenerateType.HeightMap)
        {
            display.DrawHeightMap(heightMap, mapSize);
        }
        else if (generateType == GenerateType.ColoredHeightMap)
        {
            display.DrawColoredHeightMap(heightMap, colors, mapSize);
        }
        else if (generateType == GenerateType.TerrainMesh)
        {
            display.DrawTerrainMesh(terrainMesh, terrainTexture);
        }
    }
}

// Struct to quickly color vertices based on terrain type
// Very quick and inefficient implementation, but useful to
// see colors
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
