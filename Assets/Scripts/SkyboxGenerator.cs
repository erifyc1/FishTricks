using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SkyboxGenerator : MonoBehaviour
{
    private Texture2D heightmap;

    private GameObject[] children;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private int tileWidth = 250, tileLength = 250;

    [SerializeField] private string heightmapFileName;
    [SerializeField] private Material material;
    [GradientUsageAttribute(true)]
    [SerializeField] private Gradient terrainGradient;
    [SerializeField] private float vertexVariance, tileSpacing = 4f, mapHeight;

    void Start()
    {
        vertexVariance = Mathf.Clamp(vertexVariance, 0f, 1f);

        heightmap = Resources.Load("Heightmaps/" + heightmapFileName) as Texture2D;

        GenerateMesh();
    }

    void GenerateMesh()
    {
        colors = new Color[(tileWidth + 1) * (tileLength + 1)];
        vertices = new Vector3[(tileWidth + 1) * (tileLength + 1)];

        for (int i = 0, z = 0; z <= tileLength; z++)
        {
            for (int x = 0; x <= tileWidth; x++)
            {
                float widthVariance = (x == 0 || x == tileWidth || z == 0 || z == tileLength) ? 0f : Random.Range(-vertexVariance * tileSpacing, vertexVariance * tileSpacing);
                float lengthVariance = (x == 0 || x == tileWidth || z == 0 || z == tileLength) ? 0f : Random.Range(-vertexVariance * tileSpacing, vertexVariance * tileSpacing);

                float y = heightmap.GetPixel(x, z).grayscale * mapHeight * 50f;
                
                colors[i] = terrainGradient.Evaluate(Mathf.Clamp(y / (mapHeight * 50f), 0f, 1f));
                vertices[i] = new Vector3((x * tileSpacing) - (tileWidth * tileSpacing / 2.0f) + widthVariance, y, (z * tileSpacing) - (tileLength * tileSpacing / 2.0f) + lengthVariance);

                i++;
            }
        }

        triangles = new int[tileWidth * tileLength * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < tileLength; z++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + tileWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + tileWidth + 1;
                triangles[tris + 5] = vert + tileWidth + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = material;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}