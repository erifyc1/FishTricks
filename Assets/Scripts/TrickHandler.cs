using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class TrickHandler : MonoBehaviour
{
    CapsuleCollider fishCollider;
    PlayerController PC;
    AudioController AC;
    UIManager ui;
    GameObject cam;

    Text bestStreakText; // most points in one sequence // highscore
    Text currentStreakText; // current number of tricks in current airborne state // currentTrickScore
    Text currentMultiplierText; // point multiplier // currentcombo
    Text totalScoreText; // all score for whole time // totalscore

    public int playerXAxis;
    public int playerYAxis;
    public int playerZAxis;
    public int playerQEAxis;
    public float playerZRotationSpeed;

    public int totalScore = 0;
    public int currentMultiplier = 1;
    public int currentStreak = 0;
    public int bestStreak = 0;

    private List<KeyCode> keyList = new List<KeyCode>();
    public bool midTrick;
    private bool multiplierScaled = true;

    public bool trickReset = true;

    private bool scoresActive = false;

    public GameObject trickTextHolderObject;

    /*  W  A  S  D  E  Q  */
    public int[] KeyRolls = new int[] { 0, 0, 0, 0, 0, 0 }; // num of complete rolls
    public int[] FrameCounts = new int[] { 0, 0, 0, 0, 0, 0 }; // number of frames held down button
    public int[] keysDown = new int[] { 0, 0, 0, 0, 0, 0 }; // which keys are pressed

    void Start()
    {

        //list
        keyList = MakeKeyList();

        //get components
        PC = GetComponent<PlayerController>();
        AC = GameObject.FindGameObjectWithTag("Camera").transform.GetChild(0).gameObject.GetComponent<AudioController>();
        fishCollider = transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        cam = GameObject.FindGameObjectWithTag("Camera");

        //text elements // renamed and reordered in new version
        bestStreakText = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Text>();
        currentStreakText = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<Text>();
        totalScoreText = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Text>();
        currentMultiplierText = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Text>();
        bestStreakText.text = "Best Streak: " + bestStreak;
        currentMultiplierText.text = "Point Multiplier: " + currentMultiplier;
        totalScoreText.text = "Total Score: " + totalScore;
        currentStreakText.text = "Current Streak: " + currentStreak;

        currentMultiplier = 1;


    }

    private int KeyToNumeric(string key)
    {
        switch (key)
        {
            case "W":
                return 0;
            case "A":
                return 1;
            case "S":
                return 2;
            case "D":
                return 3;
            case "E":
                return 4;
            case "Q":
                return 5;
            default:
                return -1;
        }
    }
    private string NumericToKey(int num)
    {
        switch (num)
        {
            case 0:
                return "W";
            case 1:
                return "A";
            case 2:
                return "S";
            case 3:
                return "D";
            case 4:
                return "E";
            case 5:
                return "Q";
            default:
                return "";
        }
    }
    void FixedUpdate()
    {
        if (PC.StateCheck() == "Above")
        {
            for (int keyPress = 0; keyPress < keysDown.Length; keyPress++) // if key is down, increment the frame count variable at that position
            {
                if (keysDown[keyPress] == 1)
                {
                    FrameCounts[keyPress]++;
                }
            }
        }
    }
    void Update()
    {
        if (PC.inWaterBefore)
        {
            if (!scoresActive)
            {
                scoresActive = true;
                GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                bestStreakText.gameObject.SetActive(true);
                currentMultiplierText.gameObject.SetActive(true);
                totalScoreText.gameObject.SetActive(true);
                currentStreakText.gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.GetChild(5).gameObject.SetActive(true);
            }
            if (PC.StateCheck() == "Above")
            {
                for (int i = 0; i < 6; i++)
                {
                    TrickFlipCalc(i);
                }

                if (PC.landTimer >= 0.5f)
                {
                    FrameCounterClear();
                }
            }
            foreach (KeyCode playerInput in keyList)
            {
                int keycase = Input.GetKey(playerInput) ? 1 : 0;
                keysDown[KeyToNumeric(playerInput.ToString())] = keycase;
            }

            if (PC.StateCheck() == "Submerged" && PC.waterEntry == true)
            {
                int nameRolls = 0;
                foreach (int num in KeyRolls)
                {
                    nameRolls += num;
                }
                string trickName = TrickNameBuilder();
                if (trickName != "")
                {
                    if (GameObject.FindGameObjectsWithTag("TTH") != null)
                    {
                        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("TTH"))
                        {
                            Destroy(obj);
                        }
                    }
                    GameObject trickTextGO = Instantiate(trickTextHolderObject);
                    trickTextGO.transform.SetParent(cam.transform);
                    trickTextGO.transform.position = cam.transform.position;
                    trickTextGO.transform.rotation = cam.transform.rotation;
                    TextHandler trickTextHandler = trickTextGO.GetComponent<TextHandler>();
                    if (nameRolls != 0)
                    {
                        trickTextHandler.textString = trickName;
                    }
                }
                FullListClear();
                PC.waterEntry = false;
            }
            UpdateUI();
            TutorialCheck();

        }
    }




    private string TrickNameBuilder()
    {
        string[] numofFlip = new string[9] { "", " double", " triple", " quadruple", " quintuple", " sextuple", " septuple", " octuple", " nonuple" };
        string[] adj = new string[12] { "Clean", "Bold", "Crisp", "Nice", "Elegant", "Graceful", "Wonderful", "Decisive", "Dazzling", "Stylish", "Masterful", "Fantastic" };

        string flipname = "";

        int x = KeyRolls[0] == KeyRolls[2] ? 0 : KeyRolls[0] > KeyRolls[2] ? 1 : -1;
        int y = KeyRolls[3] == KeyRolls[1] ? 0 : KeyRolls[3] > KeyRolls[1] ? 1 : -1;
        int z = KeyRolls[4] == KeyRolls[5] ? 0 : KeyRolls[4] > KeyRolls[5] ? 1 : -1;

        int[] dirRolls = new int[3] { x, y, z };
        bool[] modules = new bool[3];

        if (dirRolls[0] == 1)
        {
            if (KeyRolls[0] - 1 < 9)
            {
                flipname += numofFlip[KeyRolls[0] - 1] + " frontflip";
                modules[0] = true;
            }
        }
        else if (dirRolls[0] == -1)
        {
            if (KeyRolls[0] - 1 < 9)
            {
                flipname += numofFlip[KeyRolls[2] - 1] + " backflip";
                modules[0] = true;
            }
        }

        if (dirRolls[1] == 1)
        {
            flipname += Transition(modules, 1);
            flipname += " " + KeyRolls[3]*360 + "\u00B0 right spin";
            modules[1] = true;
        }
        else if (dirRolls[1] == -1)
        {
            flipname += Transition(modules, 1);
            flipname += " " + KeyRolls[1]*360 + "\u00B0 left spin";
            modules[1] = true;
        }

        if (dirRolls[2] == 1)
        {
            flipname += Transition(modules, 2);
            flipname += " " + KeyRolls[4] * 360 + "\u00B0 right roll";
            modules[2] = true;
        }
        else if (dirRolls[2] == -1)
        {
            flipname += Transition(modules, 2);
            flipname += " " + KeyRolls[5] * 360 + "\u00B0 left roll";
            modules[2] = true;
        }

        int adjlevel = 0;
        for (int i=0; i<3; i++)
        {
            if (modules[i]) adjlevel++;
        }
        if (!(adjlevel == 0))
        {
            flipname = adj[Random.Range(1, 4) * adjlevel -1] + flipname + "!";
        }

        //Debug.Log(flipname);
        return flipname;
    }
    private string Transition(bool[] mods, int spot)
    {
        string transition = "";
        switch (spot) {
            case 0:
                break;
            case 1:
                if (mods[0]) transition = " with a";
                break;
            case 2:
                if (mods[0] && !mods[1] || !mods[0] && mods[1]) transition = " with a";
                if (mods[0] && mods[1]) transition = " and a";
                break;
        }
        return transition;
    }


    private void TrickFlipCalc(int direction)
    {
        float lengthToTravel = (fishCollider.height * 1.2f * Mathf.PI);
        float lengthTravelled = FrameCounts[direction] * PC.rotationStep;
        if (lengthTravelled >= lengthToTravel)
        {
            FrameCounts[direction] -= Mathf.FloorToInt(lengthTravelled / PC.rotationStep);
            AddScoreGeneral(1, false, true, true);
            KeyRolls[direction] += 1;
        }
    }


    private List<KeyCode> MakeKeyList()
    {
        List<KeyCode> keyList = new List<KeyCode>();

        keyList.Add(KeyCode.E);
        keyList.Add(KeyCode.Q);
        keyList.Add(KeyCode.W);
        keyList.Add(KeyCode.S);
        keyList.Add(KeyCode.A);
        keyList.Add(KeyCode.D);

        return keyList;
    }



    public void UpdateUI()
    {
        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
        }
        if (System.Convert.ToInt32(bestStreakText.text.Split(' ')[2]) != bestStreak)
        {
            bestStreakText.text = "Best Streak: " + bestStreak;
        }
        if (System.Convert.ToInt32(currentMultiplierText.text.Split(' ')[2]) != currentMultiplier)
        {
            currentMultiplierText.text = "Point Multiplier: " + currentMultiplier;
        }
        if (System.Convert.ToInt32(totalScoreText.text.Split(' ')[2]) != totalScore)
        {
            totalScoreText.text = "Total Score: " + totalScore;
        }
        if (System.Convert.ToInt32(currentStreakText.text.Split(' ')[2]) != currentStreak)
        {
            currentStreakText.text = "Current Streak: " + currentStreak;
        }
        
    }
    public void AddScoreGeneral(int amount, bool fixedScore, bool increaseMultiplier, bool increaseStreak)
    {
        AC.PlaySFX(3, false);
        if (fixedScore)
        {
            totalScore += amount;
        }
        else
        {
            totalScore += amount * currentMultiplier;
            
        }

        if (increaseMultiplier)
        {
            if (multiplierScaled)
            {
                currentMultiplier++;
                multiplierScaled = false;
            }
            else
            {
                multiplierScaled = true;
            }
        }
        if (increaseStreak)
        {
            currentStreak += 1;
        }
    }
    
    public void TutorialCheck()
    {
        if (ui.tutorialActive && ui.currentStage == 5 && (KeyRolls[0] > 0))
        {
            ui.TaskCompleted(5);
        }
        if (ui.tutorialActive && ui.currentStage == 6 && (KeyRolls[2] > 0))
        {
            ui.TaskCompleted(6);
        }
        if (ui.tutorialActive && ui.currentStage == 7 && (KeyRolls[1] > 0 || KeyRolls[3] > 0))
        {
            ui.TaskCompleted(7);
        }
        if (ui.tutorialActive && ui.currentStage == 8 && (KeyRolls[4] > 0 || KeyRolls[5] > 0))
        {
            ui.TaskCompleted(8);
        }
    }


    
    private void RollCounterClear()
    {
        for (int i = 0; i < 6; i++)
        {
            KeyRolls[i] = 0;
        }
    }

    private void FrameCounterClear()
    {
        for (int i = 0; i < 6; i++)
        {
            FrameCounts[i] = 0;
        }
    }


    private void FullListClear()
    {
        FrameCounterClear();
        RollCounterClear();
        currentStreak = 0;
        currentMultiplier = 1;
    }

























    /* -----------------------------------------------------
     * BUILD CODE ABOVE HERE, BELOW IS OLD CODE FOR STORAGE
     * 
    public class Trick
    {
        public string trickName;
        public int scoreValue;
        public string input;
        public Animator playerAnimatorController;
        public string animatorTriggerName;

        public Trick(string TrickName, int ScoreValue, string Input, string AnimatorTriggerName)
        {
            trickName = TrickName;
            scoreValue = ScoreValue;
            input = Input;
            playerAnimatorController = GameObject.Find("Player").transform.GetChild(0).GetComponent<Animator>();
            animatorTriggerName = AnimatorTriggerName;
        }
    }

    public class InputSeries
    {
        public string input;
        public float time;

        public InputSeries(string Input, float Time)
        {
            input = Input;
            time = Time;
        }
    }
    private void TrickCheckV2Update()
    {
        //Debug.Log(PC.currentState);
        //Debug.Log(trickExecutionLimit);
        if (Input.anyKeyDown && (PC.StateCheck() == "Above"))
        {
            trickSB.Append(Input.inputString);
            //Debug.Log(trickSB.ToString());
            trickExecutionTimer = 0;
        }

        if (PC.StateCheck() == "Above" && trickExecutionTimer >= trickExecutionLimit)
        {
            TrickCheckV2(trickSB.ToString());
            //Debug.Log(trickSB.ToString());
            //Debug.Log("Timer Complete");
            //Debug.Log(trickExecutionLimit);
            trickSB.Clear();
            trickExecutionTimer = 0;
        }
        else if (PC.StateCheck() == "Above")
        {
            trickExecutionTimer += Time.deltaTime;
            //Debug.Log(trickExecutionTimer + " | " + trickExecutionLimit);
        }

        if (PC.StateCheck() == "Submerged")
        {
            trickSB.Clear();
            UpdateScoreWhole();
            trickExecutionTimer = 0;
            trickScoreCounter = 0;
            trickComboCounter = 1;
        }

        if (playerAnimation.isPlaying)
        {
            midTrick = true;
        }
        else
        {
            midTrick = false;
        }
    }
    private void TrickCheckV2(string playerInput)
    {
        foreach (Trick playerTrick in trickList)
        {
            if (playerInput == playerTrick.input)
            {
                //playerAnimator.SetTrigger(playerTrick.animatorTriggerName);
                //StartCoroutine(AnimationChain(playerTrick, playerTrick.animatorTriggerName));
                foreach (AnimationState state in playerAnimation)
                {
                    //Debug.Log(state.name);
                    if (state.name == playerTrick.animatorTriggerName && (PC.StateCheck() != "Submerged"))
                    {
                        playerAnimation.CrossFadeQueued(playerTrick.animatorTriggerName);

                    }
                }
                //Debug.Log(playerAnimation.GetClip(playerTrick.animatorTriggerName).length);
                //AC.DelayPlayOneShots(playerAnimation.GetClip(playerTrick.animatorTriggerName).length, 2);
                AC.scoreSoundIterations++;
                AC.PlaySFXOneShot(2);


                trickScoreCounter += playerTrick.scoreValue * trickComboCounter;
                trickCounter += playerTrick.scoreValue * trickComboCounter;
                UpdateScore();
                if (comboInc)
                {
                    trickComboCounter++;
                    comboInc = !comboInc;
                }
                else
                {
                    comboInc = !comboInc;
                }
                Debug.Log(playerTrick.trickName + " Completed");

            }
        }
    }*/
}





