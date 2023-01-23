using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MidpointCustomTerrian : MonoBehaviour
{
    public Terrain terrain;
    public GameObject terrainClone;
    private TerrainData terrainData;
    private float HeightMin;
    private float HeightMax;
    private float HeightDampener;
    private float Roughness;
    private int smoothAmount;
    private int plants;

    public Button ResetTerrain;
    public Button SetTerrain;
    public Button Smooth;
    public Button Convert2Mesh;
    public Button Convert2Terrain;
    public Slider HeightMinS;
    public Slider HeightMaxS;
    public Slider HeightDampenerS;
    public Slider RoughnessS;
    public Slider SmoothS;
    public Slider Plants;
    public Dropdown ChooseWays;

    public Slider sandS;
    public Slider groundS;
    public Slider snowS;


    public GameObject tree;
    public GameObject tree1;
    public GameObject tree2;
    [SerializeField ]private List<GameObject> plantsList;

    public TerrainToMeshConverter convert;

    public const int mapChunkSize = 241;

    float sandLastFrame;
    float groundLastFrame;
    float snowLastFrame;
    float plantsLastFrame;

    private void Start()
    {
        HeightMinS.maxValue = 0f;
        HeightMinS.minValue = -10f;
        HeightMaxS.maxValue = 10f;
        HeightMaxS.minValue = 0f;
        HeightDampenerS.maxValue = 2f;
        HeightDampenerS.minValue = 1f;
        RoughnessS.maxValue = 5f;
        RoughnessS.minValue = 1f;
        SmoothS.maxValue = 10f;
        SmoothS.minValue = 0f;
        Plants.maxValue = 300;
        Plants.minValue = 0;
        sandS.maxValue = 200;
        sandS.minValue = 0.001f;
        groundS.maxValue = 1100;
        groundS.minValue = 200;
        snowS.maxValue = 1200;
        snowS.minValue = 1100;

        sandLastFrame = sandS.value;
        groundLastFrame = groundS.value;
        snowLastFrame = snowS.value;
        plantsLastFrame = Plants.value;
    }
    private void Update()
    {
        HeightMin = HeightMinS.value;
        HeightMax = HeightMaxS.value;
        HeightDampener = HeightDampenerS.value;
        Roughness = RoughnessS.value;
        smoothAmount = (int)SmoothS.value;
        plants = (int)Plants.value;

        if(sandS .value !=sandLastFrame ||groundS .value!=groundLastFrame  ||snowS .value !=snowLastFrame)
        {
            sandLastFrame = sandS.value;
            groundLastFrame = groundS.value;
            snowLastFrame = snowS.value;
            
            GenerateAlphaMap();
            
            if (!terrain.gameObject.activeSelf)
            {
                convert.Init();
            }
        }
        if(Plants.value != plantsLastFrame)
        {
            plantsLastFrame = Plants.value;
            PlantTree();
        }
    }
    void ChooseWaysF(int i)
    {
        switch (i)
        {
            case 0:
                break;
            case 1:
                SceneManager.LoadScene(1);
                break;
        }
    }

    [System.Obsolete]
    void ResetTerrainF()
    {
        if(plantsList .Count > 0)
        {
            foreach (GameObject item in plantsList)
            {
                Destroy(item);
            }
            plantsList.Clear();
        }
        
        float[,] heightMap = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for(int x=0;x<terrainData.heightmapResolution; x++)
        {
            for(int y=0;y<terrainData .heightmapResolution; y++)
            {
                heightMap[x, y] = 0f; 
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
        GenerateAlphaMap();
        if(!terrain .gameObject .activeSelf)
        {
            convert.Init();
        }
    }

    [System.Obsolete]
    private void OnEnable()
    {
        terrainData = Terrain.activeTerrain.terrainData;
        ResetTerrain.onClick.AddListener(ResetTerrainF);
        SetTerrain.onClick.AddListener(MidPointDisplacement);
        Smooth.onClick.AddListener(SmoothF);
        Convert2Mesh.onClick.AddListener(Convert2MeshF);
        Convert2Terrain.onClick.AddListener(Convert2TerrainF);
        ChooseWays.onValueChanged.AddListener(ChooseWaysF);
    }
    void Convert2MeshF()
    {
        convert.Init();
    }
    void Convert2TerrainF()
    {
        terrain.gameObject.SetActive(true);
        terrainClone.SetActive(false);
    }
    List <Vector2 > GenerateNeighbors(Vector2 pos,int width,int height)
    {
        List<Vector2> neighbors = new List<Vector2>();
        for (int x = -1; x < 2; x++)
        {
            for (int y=-1; y < 2; y++)
            {
                if (x != 0 || y != 0)
                {
                    Vector2 neighborPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1), Mathf.Clamp(pos.y + y, 0, height - 1));
                    if(!neighbors .Contains (neighborPos))
                    {
                        neighbors.Add(neighborPos);
                    }
                }
            }
        }
        return neighbors;
    }

    [System.Obsolete]
    void SmoothF()
    {
        float[,] heightMap=terrainData .GetHeights (0,0,terrainData .heightmapResolution ,terrainData .heightmapResolution);
        for(int s=0;s<smoothAmount ;s++)
        {
             for(int x=0;x<terrainData.heightmapResolution; x++)
            {
                for(int y=0;y<terrainData.heightmapResolution; y++)
                {
                    float[] Guass = new float[] { 0.147761f, 0.118318f, 0.0947416f };
                    float avgHeight = 0;
                    List<Vector2> neighbors = GenerateNeighbors(new Vector2(x, y), terrainData.heightmapResolution, terrainData.heightmapResolution);
                    foreach (Vector2 item in neighbors)
                    {
                        if(neighbors .Count < 8)
                        {
                            avgHeight += heightMap[(int)item.x, (int)item.y];
                        }
                        else
                        {
                            if ((item.x == x && item.y != y) || (item.y == y && item.x != x))
                            {
                                avgHeight += Guass[1] * heightMap[(int)item.x, (int)item.y];
                            }
                            else if (item.x != x && item.y != y)
                            {
                                avgHeight += Guass[2] * heightMap[(int)item.x, (int)item.y];
                            }
                        }
                    }
                    if(neighbors .Count < 8)
                    {
                        heightMap[x, y] = (avgHeight + heightMap[x, y]) / (neighbors.Count + 1);
                    }else
                    {
                        heightMap[x, y] = avgHeight + heightMap[x, y] * Guass[0];
                    }
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
        GenerateAlphaMap();
        if (!terrain.gameObject.activeSelf)
        {
            convert.Init();
        }
        Debug.Log("smooth done");
    }

    void GenerateAlphaMap()
    {
        float[,,] alphaMap=new float[terrainData .alphamapWidth, terrainData .alphamapHeight,4];
        for(int x=0;x<terrainData .alphamapWidth ;x++)
        {
            for(int y=0;y<terrainData .alphamapHeight ;y++)
            {
                float normX = (x-0.5f) / (terrainData.alphamapWidth - 1);
                float normY = (y-0.5f) / (terrainData.alphamapHeight - 1);
                var height = terrainData.GetInterpolatedHeight(normX, normY);

                var frac=0.5;

                if (height < 0.001f)
                {
                    alphaMap[y, x, 2] = 1.0f;
                }
                else if (height < sandS.value * 0.5f)
                {
                    alphaMap[y, x, 1] = 1.0f;
                }
                else if (height < sandS.value)
                {
                    frac = Mathf.Lerp(0, 1, (height - sandS.value * 0.5f) / (sandS.value * 0.5f));
                    alphaMap[y, x, 0] = (float)frac;
                    alphaMap[y, x, 1] = (float)(1 - frac);
                }
                else if (height < groundS.value)
                {
                    alphaMap[y, x, 0] = 1.0f;
                }
                else if (height < snowS.value)
                {
                    frac = Mathf.Lerp(0, 1, (height - groundS.value) / (snowS.value - groundS.value));
                    alphaMap[y, x, 3] = (float)frac;
                    alphaMap[y, x, 0] = (float)(1 - frac);
                }
                else
                {
                    alphaMap[y, x, 3] = 1.0f;
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, alphaMap);
        PlantTree();
    }

    [System.Obsolete]
    public void MidPointDisplacement()
    {
        
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int width = terrainData.heightmapResolution - 1;
        int squareSize = width;
        int cornerX, cornerY;
        int midX, midY;
        float heightMin = HeightMin;
        float heightMax = HeightMax;
        float heightDampener = (float)Mathf.Pow(HeightDampener, -1 * Roughness);

        int midXL, midXR, midYU, midYD;
        heightMap[0, 0] = Random.Range(0f, 0.2f);
        heightMap[0, terrainData.heightmapResolution - 1] = Random.Range(0f, 0.2f);
        heightMap[terrainData.heightmapResolution - 1, 0] = Random.Range(0f, 0.2f);
        heightMap[terrainData.heightmapResolution - 1, terrainData.heightmapResolution - 1] = Random.Range(0f, 0.2f);
        while (squareSize > 0)
        {
            #region Diamnond Step
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = x + squareSize;
                    cornerY = y + squareSize;
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    heightMap[midX, midY] = (float)((heightMap[x, y] + heightMap[cornerX, y] + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                }
            }
            #endregion
            #region Square Step
            for (int x = 0; x < width; x += squareSize)
            {
                for (int y = 0; y < width; y += squareSize)
                {
                    cornerX = x + squareSize;
                    cornerY = y + squareSize;
                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);
                    midXL = (int)(midX - squareSize);
                    midXR = (int)(midX + squareSize);
                    midYD = (int)(midY - squareSize);
                    midYU = (int)(midY + squareSize);

                    if (midXL <= 0 || midYD <= 0 || midXR >= width - 1 || midYU >= width - 1)
                    {
                        continue;
                    }

                    heightMap[midX, y] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midX, midYD] + heightMap[cornerX, y]) / 4.0f + Random.Range(heightMin, heightMax));
                    heightMap[cornerX, midY] = (float)((heightMap[midX, midY] + heightMap[cornerX, y] + heightMap[midXR, midY] + heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                    heightMap[midX, cornerY] = (float)((heightMap[midX, midY] + heightMap[x, cornerY] + heightMap[midX, midYU] + heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                    heightMap[x, midY] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midXL, midY] + heightMap[x, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));

                    //if (midXL <= 0)
                    //{
                    //    heightMap[x, midY] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[x, cornerY]) / 3.0f + Random.Range(heightMin, heightMax));
                    //}
                    //else
                    //{
                    //    heightMap[x, midY] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midXL, midY] + heightMap[x, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                    //}
                    //if (midYD <= 0)
                    //{
                    //    heightMap[midX, y] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[cornerX, y]) / 3.0f + Random.Range(heightMin, heightMax));
                    //}
                    //else
                    //{
                    //    heightMap[midX, y] = (float)((heightMap[midX, midY] + heightMap[x, y] + heightMap[midX, midYD] + heightMap[cornerX, y]) / 4.0f + Random.Range(heightMin, heightMax));
                    //}
                    //if (midYU >= width - 1)
                    //{
                    //    heightMap[midX, cornerY] = (float)((heightMap[midX, midY] + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 3.0f + Random.Range(heightMin, heightMax));
                    //}
                    //else
                    //{
                    //    heightMap[midX, cornerY] = (float)((heightMap[midX, midY] + heightMap[x, cornerY] + heightMap[midX, midYU] + heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                    //}
                    //if (midXR >= width - 1)
                    //{
                    //    heightMap[cornerX, midY] = (float)((heightMap[midX, midY] + heightMap[cornerX, y] + heightMap[cornerX, cornerY]) / 3.0f + Random.Range(heightMin, heightMax));
                    //}
                    //else
                    //{
                    //    heightMap[cornerX, midY] = (float)((heightMap[midX, midY] + heightMap[cornerX, y] + heightMap[midXR, midY] + heightMap[cornerX, cornerY]) / 4.0f + Random.Range(heightMin, heightMax));
                    //}
                }
            }
            #endregion
            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        terrainData .SetHeights (0,0,heightMap);
        GenerateAlphaMap();
        if (!terrain.gameObject.activeSelf)
        {
            convert.Init();
        }
    }
    void PlantTree()
    {
        if (plantsList.Count > 0)
        {
            foreach (GameObject item in plantsList)
            {
                Destroy(item);
            }
            plantsList.Clear();
        }
        int width = terrainData.heightmapResolution - 1;
        for (int k = 0; k < plants; k++)
        {
            int x1 = Random.Range(0, width);
            int y1 = Random.Range(0, width);
            GameObject treeClone = Instantiate(tree);
            plantsList.Add(treeClone);
            float height1 = terrainData.GetHeight(x1, y1);
            treeClone.transform.position = new Vector3(x1 * 2000 / 1025, Mathf.Clamp(height1, 0, 1200), y1 * 2000 / 1025);

            int x2 = Random.Range(0, width);
            int y2 = Random.Range(0, width);
            GameObject treeClone1 = Instantiate(tree1);
            plantsList.Add(treeClone1);
            float height2 = terrainData.GetHeight(x2, y2);
            treeClone1.transform.position = new Vector3(x2 * 2000 / 1025, Mathf.Clamp(height2, 0, 1200), y2 * 2000 / 1025);

            int x3 = Random.Range(0, width);
            int y3 = Random.Range(0, width);
            GameObject treeClone3 = Instantiate(tree2);
            plantsList.Add(treeClone3);
            float height3 = terrainData.GetHeight(x3, y3);
            treeClone3.transform.position = new Vector3(x3 * 2000 / 1025, Mathf.Clamp(height3, 0, 1200), y3 * 2000 / 1025);
        }
        foreach (var item in plantsList)
        {
            if (item.transform.position.y < sandS.value * 0.9 || item.transform.position.y > groundS .value)
            {
                Destroy(item);
            }
        }
    }
}
