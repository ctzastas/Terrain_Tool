using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomTerrainWindow : EditorWindow {

    // DEFAULT TERRAIN SETTINGS
    Terrain terrain;
    int terrainWidth = 200;
    int terrainLength = 200;
    int terrainHeight = 600;
    public int heightMapResolution = 513;
    
    // Change the name on tools window
    string filename = "My Terrain";


    [MenuItem("Tools/Custom Terrain")]
    public static void ShowWindow() {
        CustomTerrainWindow window = (CustomTerrainWindow)GetWindow(typeof(CustomTerrainWindow));
        window.titleContent = new GUIContent("Create Terrain");
        window.minSize = new Vector2(410, 400);
        window.Show();
    }

    void OnEnable() {
        Debug.Log("active");
    }

    void OnGUI() {

        EditorGUILayout.Space();
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        terrainWidth = EditorGUILayout.IntSlider("Width", terrainWidth, 5, 10000);
        EditorGUILayout.Space();
        terrainLength = EditorGUILayout.IntSlider("Length", terrainLength, 5, 10000);
        EditorGUILayout.Space();
        terrainHeight = EditorGUILayout.IntSlider("Height", terrainHeight, 5, 10000);
        EditorGUILayout.Space();
        heightMapResolution = EditorGUILayout.IntSlider("Height Map Resolution", heightMapResolution, 129, 4097);
        EditorGUILayout.Space();
        filename = EditorGUILayout.TextField("Terrain Name", filename);
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Create Terrain")) {
            CreateTerrain(filename);
        }

        // [Help] button
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        if (GUILayout.Button("Documentation")) {
            string url = "file://" + Application.dataPath + "/Help/documentation" + ".html";
            Help.ShowHelpPage(url);
        }
    }

    void CreateTerrain(string filename) {

        Selection.activeObject = null;
        TerrainData newTerrain = new TerrainData();

        newTerrain.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        newTerrain.baseMapResolution = 1024;
        newTerrain.heightmapResolution = heightMapResolution;
        newTerrain.SetDetailResolution(1024, 32);

        Directory.CreateDirectory(Application.dataPath + "/CustomTerrain");
        AssetDatabase.CreateAsset(newTerrain, "Assets/CustomTerrain/" + filename + ".asset");
        AssetDatabase.SaveAssets();
       
        Selection.activeObject = Terrain.CreateTerrainGameObject(newTerrain);
        terrain = Terrain.activeTerrains[Terrain.activeTerrains.Length - 1];
        terrain.detailObjectDistance = 200f;
        terrain.name = filename;
       
        // Adding CustomTerrain.cs to new terrain
        GameObject target = GameObject.Find(filename);
        target.AddComponent<CustomTerrain>();
    }
}