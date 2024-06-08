using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendMesh : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;

    private Vector3[] newVertices;
    private int[] newTriangles;

    private Mesh meshA;
    private Mesh meshB;
    private Mesh meshC;

    void Start()
    {
        
        meshA = objectA.GetComponent<MeshFilter>().mesh;
        meshB = objectB.GetComponent<MeshFilter>().mesh;

        
        if (meshA.vertexCount != meshB.vertexCount)
        {
            Debug.LogError("Meshes must have the same number of vertices to blend.");
            return;
        }

        
        newVertices = new Vector3[meshA.vertexCount];
        newTriangles = meshA.triangles; 

        // Blend vertices
        for (int i = 0; i < meshA.vertexCount; i++)
        {
            Vector3 start = meshA.vertices[i];
            Vector3 end = meshB.vertices[i];
            newVertices[i] = Vector3.Lerp(start, end, 0.5f);
        }

        
        GameObject newObj = new GameObject("BlendedMesh");
        newObj.AddComponent<MeshRenderer>();
        newObj.AddComponent<MeshFilter>();

        
        meshC = new Mesh();
        newObj.GetComponent<MeshFilter>().mesh = meshC;
        meshC.vertices = newVertices;
        meshC.triangles = newTriangles;
        meshC.RecalculateNormals(); 

        
        MeshRenderer meshRenderer = newObj.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    void Update()
    {
      
    }
}
