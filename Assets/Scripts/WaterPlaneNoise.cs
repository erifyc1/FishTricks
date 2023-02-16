using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlaneNoise : MonoBehaviour
{
    [SerializeField]
    private float power = 3;
    [SerializeField]
    private float scale = 1;
    [SerializeField]
    private float yScaleAdjust = .1f;
    [SerializeField]
    private float timeScale = 1;

    private float offSetX;
    private float offSetY;
    private MeshFilter mf;
    // Start is called before the first frame update
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        MakeNoise();
    }

    // Update is called once per frame
    void Update()
    {
        MakeNoise();
        offSetX += Time.deltaTime * timeScale;
        if (offSetY <= yScaleAdjust) offSetY += Time.deltaTime * timeScale;
        if (offSetY >= power) offSetY -= Time.deltaTime * timeScale;
    }

    void MakeNoise()
    {
        Vector3[] vertices = mf.mesh.vertices;

        for(int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * power;
        }

        mf.mesh.vertices = vertices;
    }

    float CalculateHeight(float x, float y)
    {
        float xCord = x * scale + offSetX;
        float yCord = y * scale + offSetY;

        return Mathf.PerlinNoise(xCord, yCord);
    }
}
