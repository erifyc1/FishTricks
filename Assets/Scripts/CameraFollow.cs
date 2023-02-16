using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject player;
    GameObject CamRef;

    [SerializeField]
    float yoffset = 5;
    [SerializeField]
    float zoffset = -10;
    [SerializeField]
    float maxyaw;
    [SerializeField]
    Quaternion rotation;
    private UIManager ui;

    bool lerping = false;

    // Start is called before the first frame update
    void Start()
    {
        CamRef = GameObject.FindGameObjectWithTag("CamRef");
        player = GameObject.FindGameObjectWithTag("Player");
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        transform.parent.position = player.transform.position;
        transform.position = player.transform.position + new Vector3(0, 0, -zoffset);
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    public void EnteredWater()
    {
        lerping = true;
    }
    public void SwitchToMainCameraView()
    {
        transform.parent.position = player.transform.position;
        transform.position = player.transform.position + new Vector3(0, 0, -zoffset);
        transform.rotation = Quaternion.Euler(0, 180, 0);

        CamRef.transform.position = player.transform.position;
        transform.parent.position = player.transform.position;
        transform.position = player.transform.position + new Vector3(0, yoffset, zoffset);
        transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ui.startScreen)
        {
            if (lerping)
            {
                transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, CamRef.transform.rotation, 4.2f * Time.deltaTime);
                if (Quaternion.Angle(transform.parent.rotation, CamRef.transform.rotation) < 0.2f)
                {
                    lerping = false;
                }
            }
            //camref positioning
            CamRef.transform.position = player.transform.position;
            float anglew = Mathf.Acos(player.transform.rotation.w) * 2;
            float angley = player.transform.rotation.y / Mathf.Sin(Mathf.Acos(player.transform.rotation.w));
            CamRef.transform.rotation = new Quaternion(0, angley * Mathf.Sin(anglew / 2), 0, Mathf.Cos(anglew / 2));

            //angle comparison
            transform.parent.position = player.transform.position;
            float angleDiff = Quaternion.Angle(transform.parent.rotation, CamRef.transform.rotation);
            float vectorangle = Vector3.SignedAngle(CamRef.transform.forward, transform.parent.forward, Vector3.up);

            //underwater correct
            if (!lerping && player.GetComponent<PlayerController>().StateCheck() == "Submerged")
            {
                if (angleDiff > maxyaw)
                {
                    transform.parent.Rotate(new Vector3(0, -Mathf.Sign(vectorangle) * (Mathf.Abs(vectorangle) - maxyaw), 0));
                }
            }
        }
        
    }
}
