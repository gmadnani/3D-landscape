using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class DiamondSquare
{


    // Quick implementation of Diamond Square algorithm to generate a height map. Code works but
    // isn't neat, efficient and doesn't produce great result
    public static float[,] GenerateHeightMap(int mapSize, int seed, float scale, float randomRange)
    {
        float[,] heightMap = new float[mapSize, mapSize];

        // Set random using a seed
        UnityEngine.Random.InitState(seed);

        float height = randomRange;

        // Used for normalization
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        // Makes sure scale can never be zero to remove division by 0 errors
        if (scale <= 0)
        {
            scale = 0.001f;
        }

        // Initialize the four corners
        for (int x = 0; x < mapSize; x += mapSize - 1)
        {
            for (int y = 0; y < mapSize; y += mapSize - 1)
            {
                heightMap[x, y] = GetRandomValue(randomRange);
            }
        }

        // Debug Purposes
        // print(heightMap[0, 0]);
        // print(heightMap[0, mapSize - 1]);
        // print(heightMap[mapSize - 1, 0]);
        // print(heightMap[mapSize - 1, mapSize - 1]);

        // Very inefficient way to initialize the four corners (deprecated)
        // heightMap[0, 0] = GetRandomValue(randomRange);
        // heightMap[0, mapSize - 1] = GetRandomValue(randomRange);
        // heightMap[mapSize - 1, 0] = GetRandomValue(randomRange);
        // heightMap[mapSize - 1, mapSize - 1] = GetRandomValue(randomRange);

        // Repeat the Diamond Square algorithm by halfing the side length of the square 
        // until we've reached a side length of 1 (i.e. can't go any smaller)
        for (int sideLength = mapSize - 1; sideLength >= 2; sideLength /= 2)
        {
            // print("Side Length: " + sideLength);
            int halfSize = sideLength / 2;

            // Diamond Step
            for (int x = 0; x < mapSize - 1; x += sideLength)
            {
                for (int y = 0; y < mapSize - 1; y += sideLength)
                {


                    // Get the corner values
                    float cornerValues =
                        heightMap[x, y]
                        + heightMap[x + sideLength, y]
                        + heightMap[x, y + sideLength]
                        + heightMap[x + sideLength, y + sideLength];

                    float avg = cornerValues / 4f;
                    avg += GetRandomValue(randomRange);

                    // print("(" + (x + halfSize) + ", " + (y + halfSize) + ")");
                    // String s1 = String.Format("Corners: ({0}, {1}), ({2}, {3}), ({4}, {5}), ({6}, {7})",
                    //     x, y, x + sideLength, y, x, y + sideLength, x + sideLength, y + sideLength);
                    // String s2 = String.Format("Heightmap ({0}, {1}) avg = {2}", x + halfSize, y + halfSize, avg);
                    // print(s1);
                    // print(s2);



                    heightMap[x + halfSize, y + halfSize] = avg;

                    // Used for normalization, i.e. to reassign the max and min heights
                    maxHeight = (avg > maxHeight) ? avg : maxHeight;
                    minHeight = (avg < minHeight) ? avg : minHeight;

                }
            }

            // print("Starting Square");

            // Square Step
            for (int x = 0; x <= mapSize - 1; x += halfSize)
            {
                for (int y = (x + halfSize) % sideLength; y <= mapSize - 1; y += sideLength)
                {
                    // Get the corner values, however during the square step, it alternates
                    // between even and odd number of points if the square step has been
                    // iterated through once, (i.e not first)

                    // Use for Debug
                    // print("X = " + x);
                    // print("Y = " + y);
                    // print("Half size = " + halfSize);

                    int topX = x;
                    int topY = y - halfSize;
                    int rightX = x + halfSize;
                    int rightY = y;
                    int bottomX = x;
                    int bottomY = y + halfSize;
                    int leftX = x - halfSize;
                    int leftY = y;


                    float cornerValues = 0;
                    int numCorners = 0;

                    // Decides which corners to add
                    if (x + halfSize <= mapSize - 1)
                    {
                        cornerValues += heightMap[rightX, rightY];
                        numCorners++;
                        // String s = String.Format("Right: ({0}, {1})", rightX, rightY);
                        // print(s);

                    }
                    if (x - halfSize >= 0)
                    {
                        cornerValues += heightMap[leftX, leftY];
                        numCorners++;
                        // String s = String.Format("Left: ({0}, {1})", leftX, leftY);
                        // print(s);
                    }

                    if (y - halfSize >= 0)
                    {
                        cornerValues += heightMap[topX, topY];
                        numCorners++;
                        // String s = String.Format("Top: ({0}, {1})", topX, topY);
                        // print(s);

                    }
                    if (y + halfSize <= mapSize - 1)
                    {
                        cornerValues += heightMap[bottomX, bottomY];
                        numCorners++;
                        // String s = String.Format("Bottom: ({0}, {1})", bottomX, bottomY);
                        // print(s);

                    }


                    float avg = cornerValues / numCorners;
                    avg += GetRandomValue(randomRange);

                    // Debug purposes
                    // String s1 = String.Format("Heightmap ({0}, {1}) avg = {2}", x, y, avg);
                    // print(s1);


                    heightMap[x, y] = avg;

                    // Used for normalization, i.e. to reassign the max and min heights
                    maxHeight = (avg > maxHeight) ? avg : maxHeight;
                    minHeight = (avg < minHeight) ? avg : minHeight;
                }
            }
            randomRange *= scale;
        }

        // Normalize the height map using InverseLerp (currently highly recommended because
        // this code is absolute garbage)
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                heightMap[x, y] = Mathf.InverseLerp(minHeight, maxHeight, heightMap[x, y]);
            }
        }


        return heightMap;
    }
    static float GetRandomValue(float maxRange)
    {
        return UnityEngine.Random.Range(-maxRange, maxRange);
    }


}

