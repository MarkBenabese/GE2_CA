using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    // Declare a Mesh variable to hold the mesh data
    Mesh mesh;

    // Declare a Vector3 array to hold the vertices of the mesh
    Vector3[] vertices;

    // Declare an integer array to hold the triangles of the mesh
    int[] triangles;

    // Declare public integer variables to control the width and depth of the mesh
    public int xSize = 20;
    public int zSize = 20;

    void Start()
    {
        // Create a new mesh and assign it to the MeshFilter component of the GameObject
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Call the CreateShape and UpdateMesh methods
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        // Create an array to hold the vertices of the mesh
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // Loop through the vertices array and set the x, y, and z coordinates of each vertex
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Generate a Perlin noise value based on the x and z coordinates
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;

                // Set the vertex position using the x, y, and z coordinates
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create an array to hold the triangles of the mesh
        triangles = new int[xSize * zSize * 6];

        // Loop through the triangles array and set the vertices that make up each triangle
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // Set the vertices that make up the current triangle
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                // Increment the vertex and triangle indices
                vert++;
                tris += 6;
            }

            // Increment the vertex index at the end of each row
            vert++;
        }
    }

    void UpdateMesh()
    {
        // Clear the current mesh data
        mesh.Clear();

        // Set the vertices and triangles of the mesh to the values stored in the vertices and triangles arrays
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate the normals of the mesh
        mesh.RecalculateNormals();
    }
}
