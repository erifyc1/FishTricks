using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void Exit()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioController>().PlaySFX(6, false);
        Application.Quit();
    }
}
