using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float skyboxMovementMultiplier = 1f;

    void Update()
    {
        float percentMovementX = (player.position.x + 500f) / 1000f;
        float percentMovementZ = (player.position.z + 500f) / 1000f;
        float bounds = 62.5f * 0.025f * skyboxMovementMultiplier;
        Vector3 relativePosition = new Vector3(
            -(bounds / 2f) + (percentMovementX * bounds),
            3000f,
            -(bounds / 2f) + (percentMovementZ * bounds)
        );
        transform.position = relativePosition;
    }
}
