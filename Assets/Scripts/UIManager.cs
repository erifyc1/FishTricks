using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject[] Screen = new GameObject[8];
    public GameObject GJ;
    private GameObject title;
    public bool tutorialActive = true;
    public int currentStage = 0;
    private bool CActive = false;
    public bool startScreen = true;
    private AudioController audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioController>();
        GJ.SetActive(false);
        title = GameObject.FindGameObjectWithTag("StartButton").transform.parent.gameObject;
        title.SetActive(true);
        tutorialActive = true;
    }

    // Update is called once per frame
    public void StartGame()
    {
        Screen[0].SetActive(true);
    }
    public void FirstTimeInWater()
    {
        GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraFollow>().SwitchToMainCameraView();
        if (tutorialActive)
        {
            TaskCompleted(0);
        }
    }
    public void TaskCompleted(int task)
    {
        if (!CActive)
        {
            CActive = true;
            StartCoroutine(G(task));
        }
    }
    IEnumerator G(int task)
    {
        if (task != 4 && task != 0)
        {
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < Screen.Length; i++)
        {
            Screen[i].SetActive(false);
        }
        if (task != 4 && task != 0)
        {
            GJ.SetActive(true);
            yield return new WaitForSeconds(2);
            GJ.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        Screen[task + 1].SetActive(true);
        currentStage = task + 1;
        CActive = false;
    }
    public void StartTutorial()
    {
        audio.PlaySFX(6, false);
        tutorialActive = true;
        Screen[0].SetActive(false);
        startScreen = false;
    }
    public void StopTutorial()
    {
        audio.PlaySFX(6, false);
        tutorialActive = false;
        for (int i = 0; i < Screen.Length; i++)
        {
            Screen[i].SetActive(false);
        }
        startScreen = false;
    }
    public void TrickIntro()
    {
        audio.PlaySFX(6, false);
        TaskCompleted(4);
    }
    public void ResetGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
}
