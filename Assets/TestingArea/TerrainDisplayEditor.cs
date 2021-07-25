using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Create an editor class to add a button to regenerate the current terrain
// Script is really just for my sanity so I don't have to press play and see
// the terrain be regenerated
[CustomEditor(typeof(TerrainGenerator))]
public class TerrainDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        // If any of the values in the inspector change and auto update
        // is enabled, regenerate the terrain
        if (DrawDefaultInspector())
        {
            if (terrainGenerator.autoUpdate)
            {
                terrainGenerator.GenerateTerrain();
            }
        }

        // Create a generate button in the inspector
        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.GenerateTerrain();
        }
    }
}
