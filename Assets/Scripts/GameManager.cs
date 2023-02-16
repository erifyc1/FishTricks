using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    bool enableFPSCounter = true;
    private float secondTimer = 0;
    private int fps = 0;
    public int collectibleCounter;
    private bool paused = false;
    PlayerController pcontrol;
    UIManager ui;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    float timescale = 1;

    public GameObject[] fishCoins;

    void Start()
    {
        pcontrol = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        collectibleCounter = 0;
        fishCoins = GameObject.FindGameObjectsWithTag("FishCoin");
    }
    

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timescale;
        FPS();
        if (Input.GetKeyDown(KeyCode.Escape) && !ui.startScreen)
        {
            if (paused)
            {
                Time.timeScale = 1;
                paused = false;
                pauseMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                paused = true;
                pauseMenu.SetActive(true);
            }
        }
    }
    public void Resume()
    {
        Time.timeScale = 1;
        paused = false;
        pauseMenu.SetActive(false);
    }
    void FPS()
    {
        if (enableFPSCounter)
        {
            secondTimer += Time.deltaTime;
            fps += 1;
            if (secondTimer >= 1)
            {
                Debug.Log(fps);
                fps = 0;
                secondTimer = 0;
            }
        }
    }
}
