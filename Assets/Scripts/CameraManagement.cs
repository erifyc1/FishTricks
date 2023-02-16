using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    private Quaternion localStartRotation;
    
    private Vector3 playerStartPos;
    private Vector3 playerPosOffset;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        localStartRotation = transform.localRotation;
        player = GameObject.Find("Player");
        playerStartPos = player.transform.position;
        playerPosOffset = playerStartPos - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if(transform.localRotation != localStartRotation)
        {
            transform.localRotation = localStartRotation;
        }

        if(playerPosOffset != player.transform.position - this.transform.position)
        {
            transform.position = player.transform.position - playerPosOffset;
        }
    }
}
