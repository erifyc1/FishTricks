using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void ToMainScene()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioController>().PlaySFX(6, false);
        GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>().StartGame();
        transform.parent.gameObject.SetActive(false);
    }
}
