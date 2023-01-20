using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class TerrainToMeshConverter : MonoBehaviour
{
    public GameObject terrainSelect;
    public GameObject terrainClone;
    public Slider vertexScale;
    private void Start()
    {
        vertexScale.maxValue = 45;
        vertexScale.minValue = 10;
        vertexScale.wholeNumbers = true; 
    }
    
    public void Init()
    {
        var terrain = terrainSelect.GetComponent<Terrain>();
        var terrainData = terrain.terrainData;
        
        int w=terrainData .heightmapResolution , h = terrainData.heightmapResolution;
        Vector3 size=terrainData .size;
        float[,,] alphaMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        Vector3 meshScale = new Vector3(size.x / (w - 1f) * vertexScale.value, 1, size.z / (h - 1f) * vertexScale.value);
        Vector2 uvScale = new Vector2(1f / (w - 1f), 1f / (h - 1f)) * vertexScale.value * (size.x / terrainData.splatPrototypes[0].tileSize.x);

        w = (w - 1) / (int)vertexScale.value + 1;
        h = (h - 1) / (int)vertexScale.value + 1;
        Vector3[] vertices = new Vector3[w * h];
        Vector2[] uvs = new Vector2[w * h];
        Vector4[] alphasWeight = new Vector4[w * h];//4 textures

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                int index = j * w + i;
                float z = terrainData.GetHeight(i * (int)vertexScale.value, j * (int)vertexScale.value);
                vertices[index] = Vector3.Scale(new Vector3(i, z, j), meshScale);
                uvs[index] = Vector2.Scale(new Vector2(i, j), uvScale);

                int i2 = (int)(i * terrainData.alphamapWidth / (w - 1f));
                int j2 = (int)(j * terrainData.alphamapHeight / (h - 1f));
                i2 = Mathf.Min(terrainData.alphamapWidth - 1, i2);
                j2 = Mathf.Min(terrainData.alphamapHeight - 1, j2);
                var alpha0 = alphaMapData[j2, i2, 0];
                var alpha1 = alphaMapData[j2, i2, 1];
                var alpha2 = alphaMapData[j2, i2, 2];
                var alpha3 = alphaMapData[j2, i2, 3];
                alphasWeight[index] = new Vector4(alpha0, alpha1, alpha2, alpha3);
            }
        }

        int[] triangles = new int[(w - 1) * (h - 1) * 6];
        int triangleIndex = 0;
        for (int i = 0; i < w - 1; i++)
        {
            for (int j = 0; j < h - 1; j++)
            {
                int a = j * w + i;
                int b = (j + 1) * w + i;
                int c = (j + 1) * w + i + 1;
                int d = j * w + i + 1;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = c;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = d;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.tangents = alphasWeight;
        mesh.RecalculateNormals();
       
        MeshRenderer mr = terrainClone.GetComponent<MeshRenderer>();
        Material mat = mr.sharedMaterial;

        for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
        {
            var sp = terrainData.splatPrototypes[i];
            mat.SetTexture("_Texture" + i, sp.texture);
        }

        terrainClone.transform.position = terrainSelect.transform.position;
        terrainClone.gameObject.layer = terrainSelect.layer;
        terrainClone.GetComponent<MeshFilter>().sharedMesh = mesh;
        terrainClone.GetComponent<MeshCollider>().sharedMesh = mesh;
        mr.sharedMaterial = mat;

        terrainClone.gameObject.SetActive(true);
        terrainSelect.SetActive(false);
    }
}

