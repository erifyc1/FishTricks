using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : MonoBehaviour
{
    private Text collectibleText;
    private GameManager GM;
    private AudioController AC;
    // Start is called before the first frame update
    void Start()
    {
        collectibleText = GameObject.Find("Canvas").transform.GetChild(5).GetComponent<Text>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        AC = GameObject.Find("CameraPivot").transform.GetChild(0).transform.GetChild(0).GetComponent<AudioController>();
        
        //collectibleText.text = "Coins: " + GM.collectibleCounter + "/" + 40;
        collectibleText.text = "<color=#ff8100>Koi</color>ns: " + GM.collectibleCounter + " / " + 40;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.tag == "Player")
        {
            GM.collectibleCounter++;
            AC.PlaySFX(9, false);
            collectibleText.text = "<color=#ff8100>Koi</color>ns: " + GM.collectibleCounter + " / " + GM.fishCoins.Length;
            Destroy(this.gameObject);
        }
    }
}
