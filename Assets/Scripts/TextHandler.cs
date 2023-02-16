using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TextHandler : MonoBehaviour
{
    private float colorRed = 1.0f;
    private float colorGreen = 0.0f;
    private float colorBlue = 1.0f;
    private char[] colorCycle = { 'r', 'g', 'b' };
    private int colorCycleNum = 3;
    private bool colorDecrease = true;
    private Camera cam;
    public TextMeshPro TMP;
    //public TextMeshProUGUI TMPUGUI;
    private float timer;
    private float lifeTime;
    public string textString;
    //public float rotSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("CameraPivot").gameObject.transform.GetChild(0).GetComponent<Camera>();
        TMP = gameObject.GetComponent<TextMeshPro>();
        transform.position += transform.forward * 2.5f + transform.up * 0.25f;
        //TMPUGUI = gameObject.GetComponent<TextMeshProUGUI>();
        TMP.SetText(textString);
        timer = 0;
        lifeTime = 5;
    }

    // Update is called once per frame
    void Update()
    {
        ColorCycle();
        
        HandleLifeCycle();
        
        //transform.LookAt(-cam.transform.position);//new Vector3(cam.transform.position.x, cam.transform.position.y, -cam.transform.position.z)
        //transform.Rotate(Vector3.up, 180);
    }

    private void ColorCycle()
    {
        //same logic as the color cycle code in the MenuButton class but this one just iterate through at increments of .05
        //instead of putting the other class onto text objects that need this method, they instead have this script since this is the only purpose they have
        switch (colorCycle[(colorCycleNum % 3)])
        {
            case 'r':
                {
                    if (colorDecrease)
                    {
                        colorRed -= .05f;
                    }
                    else
                    {
                        colorRed += .05f;
                    }

                    if (colorRed < 0.0001f && colorRed > -0.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    else if (colorRed > 0.999f && colorRed < 1.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    break;
                }
            case 'g':
                {
                    if (colorDecrease)
                    {
                        colorGreen -= .05f;
                    }
                    else
                    {
                        colorGreen += .05f;
                    }

                    if (colorGreen < 0.0001f && colorGreen > -0.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    else if (colorGreen > 0.999f && colorGreen < 1.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    break;
                }
            case 'b':
                {
                    if (colorDecrease)
                    {
                        colorBlue -= .05f;
                    }
                    else
                    {
                        colorBlue += .05f;
                    }

                    if (colorBlue < 0.0001f && colorBlue > -0.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    else if (colorBlue > 0.999f && colorBlue < 1.0001f)
                    {
                        colorDecrease = !colorDecrease;
                        colorCycleNum++;
                    }
                    break;
                }
        }
        TMP.color = new Color(colorRed, colorGreen, colorBlue);
    }

    private void HandleLifeCycle()
    {
        timer += Time.deltaTime;
        if(timer >= lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
