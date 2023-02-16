using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerController pc;
    private Material mat;

    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        pc = rb.gameObject.GetComponent<PlayerController>();
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = rb.velocity.y * 0.01f;
        if (!pc.inWaterBefore) velocity = -9.81f;
        mat.SetFloat("_Velocity", velocity);
    }
}
