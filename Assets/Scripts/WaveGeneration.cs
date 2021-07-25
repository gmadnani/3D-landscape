using UnityEngine;

public class WaveGeneration : MonoBehaviour
{
    public PointLight pointLight;

    bool waveGenerated = false;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Unlit/WaveShader"));
    }

    // Update is called once per frame
    void Update()
    {
        meshRenderer.material.SetColor("_PointLightColor", this.pointLight.color);
        meshRenderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());

        //generate wave
        if (!waveGenerated)
        {
            meshFilter.mesh = GenerateWave();
            waveGenerated = true;
        }

        if (TerrainGeneration.createdNew == false)
        {
            meshFilter.mesh = GenerateWave();
        }
    }

    //create wave based on instance of terrain
    Mesh GenerateWave()
    {
        Mesh m = new Mesh();

        float yValue = TerrainGeneration.terrainInstance.GetWaterLevel();
        int nDivisions = TerrainGeneration.terrainInstance.nDivisions;
        float nSize = TerrainGeneration.terrainInstance.nSize;

        int totalVertices = (nDivisions + 1) * (nDivisions + 1);
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[nDivisions * nDivisions * 2 * 3];

        TerrainGeneration.terrainInstance.GeneratePlane(vertices, triangles, nDivisions + 1, nSize / 2, nSize / nDivisions, yValue);

        m.vertices = vertices;
        m.triangles = triangles;

        return m;
    }
}
