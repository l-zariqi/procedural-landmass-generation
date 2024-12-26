using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor // This allows the map to be generated within the editor in Unity
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target; 

        if (DrawDefaultInspector()) // If any value is changed, then the map is also generated
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
