using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class CustomTerrain : MonoBehaviour {

    // PROPERTIES FOR TERRAIN
    public Terrain terrain;
    public TerrainData terrainData;                                // Holds all the data of terrain

    // RANDOM HEIGHT PROPERTY
    public Vector2 randomHeight = new Vector2(0, 0);           // Assigned to CustomTerrainEditor.cs

    // MIDPOINT PROPERTIES
    public float midHeightMin = -2f;
    public float midHeightMax = 2f;
    public float midRoughness = 2.0f;
    public float midHeightDampenerPower = 2.0f;

    // HEIGHT_MAP IMAGE PROPERTIES
    public Texture2D loadMapImage;                                 // Hold an image
    public Vector3 heightMapScale = new Vector3(1, 1, 1);   // For scaling your height map

    // BOOLEANS
    public bool resetValues = true;

    void OnEnable() {

        // Initialize terrain data
        terrain = GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
    }

    float[,] GetHeightMap() {

        // Get the Height Map
        if (!resetValues) {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        } 
        else {
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        }
    }

    // Set the functionality for the [Reset Terrain] button of CustomTerrainEditor.cs
    public void ResetTerrain() {

        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapResolution; x++) {
            for (int y = 0; y < terrainData.heightmapResolution; y++) {
                heightMap[x, y] = 0;
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
    }

    // Set the functionality for the [Generate Terrain] button
    public void RandomTerrain() {

        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapResolution; x++) {
            // For 3 dimensions the depth is vector z
            for (int y = 0; y < terrainData.heightmapResolution; y++) {
                // Generate values to an existing terrain with (+=) 
                heightMap[x, y] += Random.Range(randomHeight.x, randomHeight.y);
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
    }

    // [Midpoint Displacement] implementation
    /// <summary>
    /// Initialize the four corners of the height map to random values.
    /// Set the midpoints of each edge to the average of the two corners it's between, plus or minus a random amount.
    /// Set the center of the square to the average of those edge midpoints you just set, again with a random jitter.
    /// Recurse on the four squares inside this one, reducing the jitter.
    /// </summary>
    public void MidPoint() {

        float[,] heightMap = GetHeightMap();
        int width = terrainData.heightmapResolution - 1;
        int squareSize = width;
        float heightMin = midHeightMin;
        float heightMax = midHeightMax;
        float heightDampener = Mathf.Pow(midHeightDampenerPower, -1 * midRoughness);

        int cornerX, cornerY;
        int midX, midY;
        int midXLeft, midXRight, midYUp, midYDown;

        while (squareSize > 0) {
            for (int x = 0; x < width; x += squareSize) {
                for (int y = 0; y < width; y += squareSize) {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    heightMap[midX, midY] = (heightMap[x, y] + heightMap[cornerX, y] + heightMap[x, cornerY] +
                                             heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax);
                }
            }

            for (int x = 0; x < width; x += squareSize) {
                for (int y = 0; y < width; y += squareSize) {
                    cornerX = x + squareSize;
                    cornerY = y + squareSize;
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    midXRight = midX + squareSize;
                    midYUp = midY + squareSize;
                    midXLeft = midX - squareSize;
                    midYDown = midY - squareSize;

                    if (midXLeft <= 0 || midYDown <= 0 || midXRight >= width - 1 || midYUp >= width - 1) continue;

                    //Calculate the square value for the bottom side  
                    heightMap[midX, y] = (heightMap[midX, midY] + heightMap[x, y] + heightMap[midX, midYDown] +
                                          heightMap[cornerX, y]) / 4.0f + Random.Range(heightMin, heightMax);

                    //Calculate the square value for the top side   
                    heightMap[midX, cornerY] = (heightMap[x, cornerY] + heightMap[midX, midY] + heightMap[cornerX, cornerY] +
                                                heightMap[midX, midYUp]) / 4.0f + Random.Range(heightMin, heightMax);

                    //Calculate the square value for the left side   
                    heightMap[x, midY] = (heightMap[x, y] + heightMap[midXLeft, midY] + heightMap[x, cornerY] +
                                          heightMap[midX, midY]) / 4.0f + Random.Range(heightMin, heightMax);

                    //Calculate the square value for the right side   
                    heightMap[cornerX, midY] = (heightMap[midX, y] + heightMap[midX, midY] + heightMap[cornerX, cornerY] +
                                                heightMap[midXRight, midY]) / 4.0f + Random.Range(heightMin, heightMax);
                }
            }
            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    // Set the functionality for the [Load Texture] button
    public void LoadMap() {

        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapResolution; x++) {
            for (int z = 0; z < terrainData.heightmapResolution; z++) {
                // Getting the pixel color at the same dimension of heightMap[x,z] and convert this colors to gray-scale
                heightMap[x, z] = loadMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
            }
            terrainData.SetHeights(0, 0, heightMap);
        }
    }
}