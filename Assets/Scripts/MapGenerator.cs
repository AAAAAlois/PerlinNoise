using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

	public enum DrawMode { NoiseMap, ColourMap, Mesh, FalloffMap };
	public DrawMode drawMode;

	public Noise.NormalizeMode normalizeMode;

	//public const int mapChunkSize = 95;  //241-1 have many factors  239: subtract 2 border vertices
	public bool useFlatShading;

	static MapGenerator instance;
	public static int mapChunkSize
	{
		get
		{
            if (instance == null)
            {
				instance = FindObjectOfType<MapGenerator>();
            }
			if (instance.useFlatShading)
			{
				return 95;
			}
			else
			{
				return 239;
			}
		}

	}

	[Range(0, 6)]
	public int editorPreviewLOD;
	public float noiseScale;

	public int seed;
	[Range(1, 8)]
	public int octaves;
	[Range(0, 1)]
	public float persistance;
	[Range(1, 10)]
	public float lacunarity;
	public Vector2 offset;

	public bool useFalloff;

	[Range(1, 100)]
	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;  //water has no height change

	public bool autoUpdate;

	public TerrainType[] regions;

	float[,] falloffMap;

	[HideInInspector] public int lastFrameOctaves;
	[HideInInspector] public float lastFramePersistance;
	[HideInInspector] public float lastFrameLacunarity;
	[HideInInspector] public float lastFrameHeightMultiplier;
	[HideInInspector] public float lastFrameNoiseScale;
	[HideInInspector] public Vector2 lastFrameOffset;
	[HideInInspector] public int lastFrameSeed;
	[HideInInspector] public int lastFrameEditorPreviewLOD;
	[HideInInspector] public bool lastFrameUseFalloff;

	[HideInInspector] public bool isEndlessTerrain = false;



	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

	void Awake()
	{
		falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize+2);
	}


    private void Start()
    {
		lastFrameOctaves = octaves;
		lastFramePersistance = persistance;
		lastFrameLacunarity = lacunarity;
		lastFrameHeightMultiplier = meshHeightMultiplier;
    }

    public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData(Vector2.zero);

		MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
		}
		else if (drawMode == DrawMode.ColourMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD, useFlatShading), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
		}
		else if (drawMode == DrawMode.FalloffMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFalloffMap(mapChunkSize)));
		}
	}

	public void RequestMapData(Vector2 centre, Action<MapData> callback) //Action是UnityEngine.Events命名空间中定义的一种委托类型(delegate)，它可以实现不带参数和返回值的方法
	{
		ThreadStart threadStart = delegate { MapDataThread(centre, callback); };   //ThreadStart: the entrance of a thread

		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 centre, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData(centre);
		lock (mapDataThreadInfoQueue)   //when one thread reaches this point, excute code in{}, no other code can excute as well
		{
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));  //Enqueue是一个队列的入队函数,可以在队尾添加一个元素
		}
	}

	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
	{
		ThreadStart threadStart = delegate {
			MeshDataThread(mapData, lod, callback);
		};

		new Thread(threadStart).Start();
	}

	void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
	{
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod, useFlatShading);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update()
	{
		if (mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

        if (!isEndlessTerrain)
        {
			if (lastFrameOctaves != octaves || lastFramePersistance != persistance || lastFrameLacunarity != lacunarity || lastFrameHeightMultiplier != meshHeightMultiplier
			|| lastFrameNoiseScale != noiseScale || lastFrameOffset != offset || lastFrameSeed != seed || lastFrameUseFalloff != useFalloff || lastFrameEditorPreviewLOD != editorPreviewLOD)
			{

				lastFrameOctaves = octaves;
				lastFramePersistance = persistance;
				lastFrameLacunarity = lacunarity;
				lastFrameHeightMultiplier = meshHeightMultiplier;
				lastFrameNoiseScale = noiseScale;
				lastFrameOffset = offset;
				lastFrameSeed = seed;
				lastFrameUseFalloff = useFalloff;
				lastFrameEditorPreviewLOD = editorPreviewLOD;

				DrawMapInEditor();

			}
		}
		

	}

	MapData GenerateMapData(Vector2 centre)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

		Color[] colourMap = new Color[(mapChunkSize+2) * (mapChunkSize+2)];

   

		for (int y = 0; y < mapChunkSize + 2; y++)
		{
			for (int x = 0; x < mapChunkSize + 2; x++)
			{
				if (useFalloff)
				{
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
				{
					if (currentHeight >= regions[i].height)
					{
						colourMap[y * mapChunkSize + x] = regions[i].color;
					}
					else
					{
						break;
					}
				}
			}
		}


		return new MapData(noiseMap, colourMap);
	}

	void OnValidate()
	{
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}
		if (octaves < 0)
		{
			octaves = 0;
		}

		falloffMap = FallOffGenerator.GenerateFalloffMap(mapChunkSize+2);
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}

	}

}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}

public struct MapData
{
	public readonly float[,] heightMap;
	public readonly Color[] colorMap;

	public MapData(float[,] heightMap, Color[] colorMap)
	{
		this.heightMap = heightMap;
		this.colorMap = colorMap;
	}
}
