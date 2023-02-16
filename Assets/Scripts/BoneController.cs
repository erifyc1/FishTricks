using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneController : MonoBehaviour
{
    [SerializeField] private float tolerance = 0.5f;

    [HideInInspector] public bool buttonPressed = false;
    [HideInInspector] public bool animationReady = false;

    [HideInInspector] public Vector3 midRotation, tailRotation;
    [HideInInspector] public float rotateSmoothness = 2f;

    private Transform midSpine, tailSpine;

    void Start()
    {
        midSpine = transform.Find("Koi").Find("Armature").GetChild(0).GetChild(0).transform;
        tailSpine = transform.Find("Koi").Find("Armature").GetChild(0).GetChild(0).GetChild(0).transform;
    }

    void Update()
    {
        Quaternion midToRotation = Quaternion.identity;
        Quaternion tailToRotation = Quaternion.identity;

        if (buttonPressed)
        {
            midToRotation = Quaternion.Euler(midRotation.x, midRotation.y, midRotation.z);
            tailToRotation = Quaternion.Euler(tailRotation.x, tailRotation.y, tailRotation.z);
        }

        midSpine.localRotation = Quaternion.Lerp(midSpine.localRotation, midToRotation, Time.deltaTime * rotateSmoothness);
        tailSpine.localRotation = Quaternion.Lerp(tailSpine.localRotation, tailToRotation, Time.deltaTime * rotateSmoothness);

        animationReady =  1 - Mathf.Abs(Quaternion.Dot(midSpine.localRotation, Quaternion.identity)) <= tolerance
            &&  1 - Mathf.Abs(Quaternion.Dot(tailSpine.localRotation, Quaternion.identity)) <= tolerance;
    }
}
