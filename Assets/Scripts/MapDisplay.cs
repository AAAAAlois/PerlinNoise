using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    //public MeshCollider meshCollider;

    public void DrawTexture(Texture2D texture)
    {
       

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);

    }

    public void DrawMesh(MeshData meshData, Texture2D texture2D)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();  //sharedMesh: maybe generate mesh outside of game
        meshRenderer.sharedMaterial.mainTexture = texture2D;
    }

}
