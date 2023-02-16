using System.Collections;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public void RespawnFish()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(0, 30, 0);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(WaitFrame());

    }
    IEnumerator WaitFrame()
    {
        yield return new WaitForSeconds(1);
        this.gameObject.SetActive(false);
    }
}
