using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void DrawHeightMap(float[,] heightMap, int mapSize)
    {
        // Use Lerp to create a color array that will store
        // the color value for each vertices
        Color[] colors = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                colors[y * mapSize + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        Texture2D texture = new Texture2D(mapSize, mapSize);
        // Found on a tutorial to make the texture more blocky and clamp the texture
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Once the color array has been created, apply it
        texture.SetPixels(colors);
        texture.Apply();

        // Use shared material so I don't have to press play every single time
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawColoredHeightMap(float[,] heightMap, Color[] colors, int mapSize)
    {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        // Found on a tutorial to make the texture more blocky and clamp the texture
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Once the color array has been created, apply it
        texture.SetPixels(colors);
        texture.Apply();

        // Use shared material so I don't have to press play every single time
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawTerrainMesh(Mesh terrainMesh, Texture2D terrainTexture)
    {
        // Use shared mesh & material so I don't have to press play every single time
        meshFilter.sharedMesh = terrainMesh;
        meshCollider.sharedMesh = terrainMesh;
        meshRenderer.sharedMaterial.mainTexture = terrainTexture;
    }
}
