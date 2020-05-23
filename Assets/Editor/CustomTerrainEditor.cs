using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomTerrain))] [CanEditMultipleObjects]
public class CustomTerrainEditor : Editor {

    // ========== SERIALIZED PROPERTIES =========== //
    SerializedProperty randomHeight;
    SerializedProperty heightMapScale;
    SerializedProperty loadMapImage;
    SerializedProperty resetValues;
    SerializedProperty midHeightMin;
    SerializedProperty midHeightMax;
    SerializedProperty midRoughness;
    SerializedProperty midHeightDampenerPower;
    Texture2D heightMapTexture;

    // Change the name on inspector
    string filename = "My Height Map";

    // ======= BOOLEANS ======= //
    bool showRandom;
    bool showLoadMap;
    bool showMidPoint;
    bool showHeighMap;


    void OnEnable() {

        // Αssign variables from CustomTerrain.cs into this variables 
        randomHeight = serializedObject.FindProperty("randomHeight");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        loadMapImage = serializedObject.FindProperty("loadMapImage");
        resetValues = serializedObject.FindProperty("resetValues");
        midHeightMin = serializedObject.FindProperty("midHeightMin");
        midHeightMax = serializedObject.FindProperty("midHeightMax");
        midRoughness = serializedObject.FindProperty("midRoughness");
        midHeightDampenerPower = serializedObject.FindProperty("midHeightDampenerPower");

        // Get the height Map size from CustomTerrainWindow.cs values of heightMapResolution.
        CustomTerrainWindow target = new CustomTerrainWindow();
        heightMapTexture = new Texture2D(target.heightMapResolution, target.heightMapResolution, TextureFormat.ARGB32, false);
    }

    public override void OnInspectorGUI() {

        // Update all the serialized values of CustomTerrain.cs
        serializedObject.Update();

        // Link to the script CustomTerrain.cs
        CustomTerrain terrain = (CustomTerrain)base.target;

        EditorGUILayout.PropertyField(resetValues);
        EditorGUILayout.Space();

        // [Random Terrain] button
        showRandom = EditorGUILayout.BeginFoldoutHeaderGroup(showRandom, "Random Terrain");
        if (showRandom) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Generate Random Width and Height Values", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(randomHeight, new GUIContent("Random Values (X,Y)"));
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Terrain")) {
                terrain.RandomTerrain();
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // [Midpoint Displacement] button
        showMidPoint = EditorGUILayout.BeginFoldoutHeaderGroup(showMidPoint, "Midpoint Displacement");
        if (showMidPoint) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.Slider(midHeightMin,-10,10, new GUIContent("Min. Height"));
            EditorGUILayout.Space();
            EditorGUILayout.Slider(midHeightMax, 0, 10, new GUIContent("Max. Height"));
            EditorGUILayout.Space();
            EditorGUILayout.Slider(midRoughness, 0, 10, new GUIContent("Roughness"));
            EditorGUILayout.Space();
            EditorGUILayout.Slider(midHeightDampenerPower, 0, 10, new GUIContent("Height Dampener Power"));
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate terrain")) {
                terrain.MidPoint();
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // [Generate & Save Height map] Button]
        showHeighMap = EditorGUILayout.BeginFoldoutHeaderGroup(showHeighMap, "Generate & Save Texture");
        if (showHeighMap) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Generate & Save grayscale Height Map Texture (.png)", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            filename = EditorGUILayout.TextField("Filename", filename,GUILayout.Width(340));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int heighMapSize = (int)(EditorGUIUtility.currentViewWidth - 100);
            GUILayout.Label(heightMapTexture, GUILayout.Width(heighMapSize), GUILayout.Height(heighMapSize));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate Texture", GUILayout.Width(150))) {
                float[,] heightMap = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution,
                    terrain.terrainData.heightmapResolution);
                for (int y = 0; y < terrain.terrainData.heightmapResolution; y++) {
                    for (int x = 0; x < terrain.terrainData.heightmapResolution; x++) {
                        heightMapTexture.SetPixel(x, y, new Color(heightMap[x, y], heightMap[x, y], heightMap[x, y], 1));
                    }
                }
                heightMapTexture.Apply();
            }
            if (GUILayout.Button("Save Texture", GUILayout.Width(150))) {
                byte[] bytes = heightMapTexture.EncodeToPNG();
                Directory.CreateDirectory(Application.dataPath + "/Maps");
                File.WriteAllBytes(Application.dataPath + "/Maps/" + filename + ".png", bytes);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // [Load Height Map] button
        showLoadMap = EditorGUILayout.BeginFoldoutHeaderGroup(showLoadMap, "Load Texture");
        if (showLoadMap) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load grayscale Height Map Texture (.png)", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loadMapImage, new GUIContent("Import Height Map"));
            EditorGUILayout.Space(10);
            GUILayout.Label("Default Scale (512 X 512)", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(heightMapScale);
            EditorGUILayout.Space();
            if (GUILayout.Button("Load Texture")) {
                terrain.LoadMap();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // [Reset Terrain] button
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        if (GUILayout.Button("Reset Terrain")) {
            terrain.ResetTerrain();
        }

        EditorGUILayout.Space();
        // Apply changes inside serialized.Update();
        serializedObject.ApplyModifiedProperties();
    }
}