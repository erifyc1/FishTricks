using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    void OnTriggerExit(Collider other) {
        Debug.Log("This Method Works");
        if (other.tag == "Player")
        {
            Debug.Log("This tag works.");
            ps.Play();
        }
    }
}
