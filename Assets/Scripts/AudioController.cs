using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    public AudioSource[] SFX = new AudioSource[2];
    //0: Main Melody
    //1: jump out,
    //2: jump in,
    //3: score sound,
    //4: swim_1,
    //5: swim_2
    //6: button_1
    //7: Railgun
    //8: Land on Land


    //[HideInInspector]
    public int scoreSoundIterations = 0;

    void Start()
    {

    }

    public void PlaySFX(int id, bool randomPitch)
    {
        if (SFX[id])
        {
            if (randomPitch)
            {
                SFX[id].pitch = Random.Range(0.7f, 1.3f);
                SFX[id].Play();
            }
            else
            {
                SFX[id].pitch = 1;
                SFX[id].Play();
            }
        }
        else
        {
            Debug.LogError("playsound index out of bounds");
        }
    }
    public void PlaySFXOneShot(int id)
    {
        if (SFX[id])
        {
            if (scoreSoundIterations > 0)
            {
                SFX[id].PlayOneShot(SFX[id].clip);
            }
            else
            {
                StartCoroutine(DelayPlayOneShots(SFX[id].clip.length, id));
            }
        }
        else
        {
            Debug.LogError("playsound index out of bounds");
        }
    }

    public IEnumerator DelayPlayOneShots(float delay, int id)
    {
        yield return new WaitForSeconds(delay);

        SFX[id].PlayOneShot(SFX[id].clip);
        scoreSoundIterations--;
        if (scoreSoundIterations > 0)
        {
            StartCoroutine((DelayPlayOneShots(SFX[id].clip.length, id)));
        }
    }
}
