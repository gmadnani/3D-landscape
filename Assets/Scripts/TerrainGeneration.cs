using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
    //public variables
    public int nDivisions;
    public float height;
    public float nSize;
    public float reductionRate;
    public Gradient gradient;
    public PointLight pointLight;
    public static TerrainGeneration terrainInstance;
    public static bool createdNew;

    //private variables
    Vector3[] vertices;
    int totalVertices;
    float minTerrainHeight;
    float maxTerrainHeight;

    // Generates terrain when starts
    void Start()
    {
        //create terrain mesh
        MeshFilter terrainMesh = this.gameObject.GetComponent<MeshFilter>();
        terrainMesh.mesh = this.CreateTerrainMesh();

        //render the mesh
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Unlit/ColorShader"));

        //set mesh collider to the terrain mesh
        MeshCollider collider = this.gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = terrainMesh.sharedMesh;

        //set boundaries
        Boundries.instance.GetBounds();
    }

    void Awake()
    {
        //for sending the terrain instance to create wave
        terrainInstance = this;
        createdNew = true;
    }

    // Generates new terrain when space is pressed
    void Update()
    {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());

        if (Input.GetKeyDown(KeyCode.Space)) {
            createdNew = false;
            //reset max and min height
            minTerrainHeight = Mathf.Infinity;
            maxTerrainHeight = -Mathf.Infinity;

            //create new terrain mesh
            MeshFilter terrainMesh = this.gameObject.GetComponent<MeshFilter>();
            terrainMesh.mesh = this.CreateTerrainMesh();

            //render mesh
            renderer.material = new Material(Shader.Find("Unlit/ColorShader"));

            //set mesh collider to new terrain mesh
            MeshCollider collider = this.gameObject.GetComponent<MeshCollider>();
            collider.sharedMesh = terrainMesh.sharedMesh;

            terrainInstance = this;
            
            //camera reposition
            Flying.instance.LateActivation();
        }
    }

    // Function to create terrain mesh
    Mesh CreateTerrainMesh()
    {
        Mesh m = new Mesh();

        //build vertices, colors and triangle array
        totalVertices = (nDivisions + 1) * (nDivisions + 1);
        vertices = new Vector3[totalVertices];
        int[] triangles = new int[nDivisions * nDivisions * 2 * 3];
        Color[] colors = new Color[totalVertices];
        Vector3[] normals = new Vector3[totalVertices];

        float halfSize = nSize / 2;
        float divisionSize = nSize / nDivisions;

        // number of vertices in a single row/column
        int numVerticesRow = nDivisions + 1;

        // generate flat plane by going through every division
        GeneratePlane(vertices, triangles, numVerticesRow, halfSize, divisionSize, 0);

        // setup heightmap
        float heightChange = height;
        float[,] heightMap = new float[numVerticesRow, numVerticesRow];

        // set random y values for the 4 corners of the heightmap
        GenerateInitialPoints(heightMap, heightChange, numVerticesRow);

        // create heightmap with diamond sqaure algorithm
        DiamondSquareAlgo(heightMap, heightChange);

        // move heightmap to y values of vertices array in mesh
        TransferToMesh(heightMap, numVerticesRow);

        //update min and max height of terrain
        UpdateMaxMinTerrain(vertices);

        //color terrain based on gradient
        ColorTerrain(colors);

        //set into mesh
        m.vertices = vertices;
        m.colors = colors;
        m.normals = normals;
        m.triangles = triangles;

        // calculate normals and recalculate bounds 
        m.RecalculateBounds();
        m.RecalculateNormals();

        return m;
    }

    // transfer heightmap data into vertices array in mesh
    void TransferToMesh(float[,] heightMap, int numVerticesRow)
    {
        int index = 0;
        for (int i = 0; i < numVerticesRow; i++)
        {
            for (int j = 0; j < numVerticesRow; j++)
            {
                vertices[index].y = heightMap[i, j];
                index++;
            }
        }
    }

    // Perform diamond square algorithm to create heightmap
    void DiamondSquareAlgo(float[,] heightMap, float heightChange)
    {
        // For loop to go through the square. After doing the diamond and square
        // step, the size of the square is halved, which is the sideLength. Halving
        // the side length for the first time results in sideLength to be rounded
        // down to an integer, since sideLength is an integer. E.g. 5/2 = 2 not 2.5.
        for (int sideLength = nDivisions; sideLength >= 2; sideLength /= 2)
        {
            int halfRegion = sideLength / 2;

            // Diamond Step
            // The Diamond Step is done in regions that is determined by the
            // size square, i.e. the smaller the square size, the more the regions
            // resulting in more iterations of both x and y
            for (int x = 0; x < nDivisions; x += sideLength)
            {
                for (int y = 0; y < nDivisions; y += sideLength)
                {
                    float cornerValues =
                        heightMap[x, y] // Top Left
                        + heightMap[x + sideLength, y] // Top Right
                        + heightMap[x, y + sideLength] // Bottom Left
                        + heightMap[x + sideLength, y + sideLength]; // Bottom Right

                    float avg = cornerValues / 4f;
                    avg += GenerateRandomLength(heightChange);

                    // The middle of the square is (x + half the square size,
                    // y + half the square size)
                    heightMap[x + halfRegion, y + halfRegion] = avg;
                }
            }

            // Square Step
            // Since the Diamond Square Algorithm creates a height map, we can use (x, y)
            // to assign the values. The x values will be incremented with half the square 
            // size as the x-distance between square points, is half the square size
            for (int x = 0; x <= nDivisions; x += halfRegion)
            {
                // The y values on the other hand will be incremented by the size of the square
                // as the y-distance between the square points is the size of the square. However, 
                // there is a pattern on where the y value starts for each x value, which we can 
                // determine the y value using the formula (x + half the square size) %  sideLength
                for (int y = (x + halfRegion) % sideLength; y <= nDivisions; y += sideLength)
                {
                    // x, y co-ordinate of the point directly above the square point
                    int topX = x;
                    int topY = y - halfRegion;

                    // x, y co-ordinate of the point to the right of the square point
                    int rightX = x + halfRegion;
                    int rightY = y;

                    // x, y co-ordinate of the point directly below the square point
                    int bottomX = x;
                    int bottomY = y + halfRegion;
                    
                    // x, y co-ordinate of the point to the left of the square point
                    int leftX = x - halfRegion;
                    int leftY = y;

                    float cornerValues = 0;
                    int numCorners = 0;

                    // Decides which corners to add
                    // Only add points that are present in the square, in respect
                    // to the square point. E.g. Square point (0, 1) doesn't have 
                    // a right point, which would be (-0, 1), as it is out of bounds

                    // If the square point has a point to the right of itself
                    // add the right point
                    if (x + halfRegion <= nDivisions)
                    {
                        cornerValues += heightMap[rightX, rightY];
                        numCorners++;
                    }

                    // If the square point has a point to the left of itself
                    // add the left point
                    if (x - halfRegion >= 0)
                    {
                        cornerValues += heightMap[leftX, leftY];
                        numCorners++;
                    }

                    // If the square point has a point above
                    // add the top point
                    if (y - halfRegion >= 0)
                    {
                        cornerValues += heightMap[topX, topY];
                        numCorners++;
                    }

                    // If the square point has a point below
                    // add the bottom point
                    if (y + halfRegion <= nDivisions)
                    {
                        cornerValues += heightMap[bottomX, bottomY];
                        numCorners++;
                    }

                    float avg = cornerValues / numCorners;
                    avg += GenerateRandomLength(heightChange);
                    heightMap[x, y] = avg;
                }
            }
            heightChange *= reductionRate;
        }
    }

    //color the terrain based on height
    void ColorTerrain(Color[] colors)
    {
        for (int i = 0; i < totalVertices; i++)
        {
            float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);

            // give terrain static color
            colors[i] = gradient.Evaluate(height);

            // give terrain color between ranges for each region
            //color green
            if (height > 0.35f && height <= 0.67f)
            {
                colors[i] = new Color(Random.Range(0.2f, 0.3f), Random.Range(0.47f, 0.55f), Random.Range(0.2f, 0.25f));
            }
            //color grey
            else if (height > 0.67f && height <= 0.78f)
            {
                float rgb = Random.Range(0.4f, 0.47f);
                colors[i] = new Color(rgb, rgb, rgb);
            }
            //color white
            else if (height > 0.78f && height <= 1f)
            {
                float rgb = Random.Range(0.95f, 1f);
                colors[i] = new Color(rgb, rgb, rgb);
            }
            //color orange
            else if (height > 0.2f && height <= 0.35f)
            {
                colors[i] = new Color(Random.Range(0.7f, 0.78f), Random.Range(0.47f, 0.57f), Random.Range(0.08f, 0.2f));
            }
        }
    }

    // Get water level height for wave generation
    public float GetWaterLevel()
    {
        float waterHeight = 0.22f;
        float waterLevel = (maxTerrainHeight - minTerrainHeight) * waterHeight + minTerrainHeight;
        return waterLevel;
    }

    //fill vertices and traingles array to create flat plane based on size
    public void GeneratePlane(Vector3[] vertices, int[] triangles, int numVerticesRow, float halfSize, float divisionSize, float yValue)
    {
        int triIndex = 0;
        for (int i = 0; i < numVerticesRow; i++)
        {
            for (int j = 0; j < numVerticesRow; j++)
            {
                vertices[i * numVerticesRow + j] = new Vector3(-halfSize + j * divisionSize, yValue, halfSize - i * divisionSize);

                //last vertex for each row or column should not have a triangle
                if (i < nDivisions && j < nDivisions)
                {
                    int topLeft = i * (numVerticesRow) + j;
                    int bottomLeft = (i + 1) * (numVerticesRow) + j;

                    //form triangles
                    triangles[triIndex] = topLeft;
                    triangles[triIndex + 1] = topLeft + 1;
                    triangles[triIndex + 2] = bottomLeft + 1;

                    triangles[triIndex + 3] = topLeft;
                    triangles[triIndex + 4] = bottomLeft + 1;
                    triangles[triIndex + 5] = bottomLeft;

                    triIndex += 6;
                }
            }
        }
    }

    // generate initial corner points in heightmap
    void GenerateInitialPoints(float[,] heightMap, float heightChange, int numVerticesRow)
    {
        heightMap[0, 0] = GenerateRandomLength(heightChange);
        heightMap[0, numVerticesRow - 1] = GenerateRandomLength(heightChange);
        heightMap[numVerticesRow - 1, 0] = GenerateRandomLength(heightChange);
        heightMap[numVerticesRow - 1, numVerticesRow - 1] = GenerateRandomLength(heightChange);
    }

    // update the max and min height of terrain
    void UpdateMaxMinTerrain(Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y < minTerrainHeight)
            {
                minTerrainHeight = vertices[i].y;
            }
            else if (vertices[i].y > maxTerrainHeight)
            {
                maxTerrainHeight = vertices[i].y;
            }
        }
    }

    float GenerateRandomLength(float num)
    {
        return Random.Range(-num, num);
    }

}
