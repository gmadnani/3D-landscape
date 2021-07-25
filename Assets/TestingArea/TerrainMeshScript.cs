using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMeshScript
{
    // Used to create the terrain mesh. Mainly to help me stop pressing play as its 
    // getting rly annoying especially to see minor changes. Will implement with start
    // later on, but right now can't be bothered
    public static Mesh CreateTerrainMesh(float[,] heightMap, float heightMultiplier, int mapSize, AnimationCurve heightCurve)
    {
        Vector3[] vertices = new Vector3[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
        Vector2[] uvs = new Vector2[mapSize * mapSize];

        // Used to center the mesh. Starting from the top left
        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / 2f;

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                uvs[vertexIndex] = new Vector2(x / (float)mapSize, y / (float)mapSize);

                // We skip the right and bottom edge
                if (x < mapSize - 1 && y < mapSize - 1)
                {
                    AddTriangle(ref triangles, ref triangleIndex, vertexIndex, vertexIndex + mapSize + 1, vertexIndex + mapSize);
                    AddTriangle(ref triangles, ref triangleIndex, vertexIndex + mapSize + 1, vertexIndex, vertexIndex + 1);

                    // Creates 1st Triangle
                    // triangles[triangleIndex] = vertexIndex;
                    // triangles[triangleIndex + 1] = vertexIndex + mapSize + 1;
                    // triangles[triangleIndex + 2] = vertexIndex + mapSize;
                    // // Creates 2nd Triangle
                    // triangles[triangleIndex + 3] = vertexIndex + mapSize + 1;
                    // triangles[triangleIndex + 4] = vertexIndex;
                    // triangles[triangleIndex + 5] = vertexIndex + 1;
                    // triangleIndex += 6;
                }

                vertexIndex++;
            }
        }
        Mesh mesh = new Mesh();
        mesh.name = "Terrain Mesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    // Add triangle to the triangles array given 3 vertices and the 
    // triangle index
    static void AddTriangle(ref int[] triangles, ref int triangleIndex, int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }
}
