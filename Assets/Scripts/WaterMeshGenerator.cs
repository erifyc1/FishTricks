using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterMeshGenerator : MonoBehaviour
{
    private Vector2[] variance;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    [Header("Generation Settings")]
    [SerializeField] private int tileWidth = 10;
    [SerializeField] private int tileLength = 10;
    [SerializeField] private float averageSpacing = 1f;
    [SerializeField] private float vertexVariance = 0.25f;

    [Header("Water Settings")]
    [SerializeField] private Material material;
    [SerializeField] private float windStrength = 0f;
    [SerializeField] private float waveDirection = 0f;
    [SerializeField] private float turbulence = 0.5f;
    [SerializeField] private float waveHeight = 0.25f;
    private float widthTime, lengthTime;

    void Start()
    {   
        tileWidth = Mathf.Max(1, tileWidth);
        tileLength = Mathf.Max(1, tileLength);
        averageSpacing = Mathf.Abs(averageSpacing);
        vertexVariance = Mathf.Clamp(vertexVariance, 0f, 1f);
        vertexVariance = Mathf.Abs(vertexVariance);
        waveDirection = Mathf.Clamp(waveDirection, 0f, 360f);
    
        GetComponent<MeshFilter>().mesh = new Mesh();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = material;
        mr.shadowCastingMode = ShadowCastingMode.Off;

        CreateMesh();
    }

    void Update()
    {
        widthTime += Time.deltaTime * Mathf.Sin(waveDirection * Mathf.PI / 180f) * windStrength;
        lengthTime += Time.deltaTime * Mathf.Cos(waveDirection * Mathf.PI / 180f) * windStrength;

        windStrength = Mathf.Max(0f, windStrength);
        waveHeight = Mathf.Max(0f, waveHeight);
        waveDirection = Mathf.Clamp(waveDirection, 0f, 360f);

        RuntimeMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        variance = new Vector2[(tileWidth + 1) * (tileLength + 1)];
        for (int i = 0, z = 0; z <= tileLength; z++)
        {
            for (int x = 0; x <= tileWidth; x++)
            {
                float widthVariance = Random.Range(-vertexVariance * averageSpacing, vertexVariance * averageSpacing);
                float lengthVariance = Random.Range(-vertexVariance * averageSpacing, vertexVariance * averageSpacing);

                variance[i] = (x == 0 || x == tileWidth || z == 0 || z == tileLength) ? new Vector2(0f, 0f) : new Vector2(widthVariance, lengthVariance);
                i++;
            }
        }
    }

    void RuntimeMesh()
    {
        vertices = new Vector3[(tileWidth + 1) * (tileLength + 1)];
        for (int i = 0, z = 0; z <= tileLength; z++)
        {
            for (int x = 0; x <= tileWidth; x++)
            {
                float y = waveHeight * Mathf.PerlinNoise((x + widthTime) * turbulence, (z + lengthTime) * turbulence);

                vertices[i] = new Vector3(
                    (x * averageSpacing) - (tileWidth * averageSpacing / 2.0f) + variance[i].x, 
                    y,
                    (z * averageSpacing) - (tileLength * averageSpacing / 2.0f) + variance[i].y);
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

        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= tileLength; z++)
        {
            for (int x = 0; x <= tileWidth; x++)
            {
                uvs[i] = new Vector2((float)x / tileWidth, (float)z / tileLength);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
    }
}
