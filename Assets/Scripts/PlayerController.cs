using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoneController))]
public class PlayerController : MonoBehaviour
{
    private BoneController boneZone;
    [SerializeField] private ParticleSystem splash;
    [SerializeField] private ParticleSystem bubbles;

    private float gravity;

    public bool inWaterBefore = false;

    public enum State
    {
        Submerged,
        Above
    }

    public State currentState;

    [Header("Speed")]

    [SerializeField]
    private float maxSpeed = 10;

    [SerializeField]
    private float speedStep = 0.02f;

    [Header("Rotation")]

    [SerializeField]
    private float maxRotationSpeed = 5;

    public float rotationStep = 0.08f;

    [Header("Gravity")]

    [SerializeField]
    private float gravityScale = 9.8f;

    [SerializeField]
    private Rigidbody rb;

	[SerializeField]
    private float jumpForce = 10f;

    private Animator anim;

    [SerializeField]
    private float smoothTime = 0.01f;

    private AudioController audio;

    private CameraFollow camFollow;

    [SerializeField]
    private Button respawnButton;

    [SerializeField]
    private UIManager ui;

    //Now no touchy. :)
    [HideInInspector] public int xAxis = 0;
    [HideInInspector] public int yAxis = 0;
    [HideInInspector] public int zAxis = 0;
    [HideInInspector] public int qeAxis = 0;
    [HideInInspector] public float currentZSpeed = 0;
    [HideInInspector] public float ZRotationSpeed = 0;
    [HideInInspector] public float currentYSpeed = 0;
    [HideInInspector] public float currentXSpeed = 0;
    [HideInInspector] public float currentQESpeed = 0;

    [HideInInspector] public float avgZSpeed = 0;
    [HideInInspector] public float avgZRotationSpeed = 0;
    [HideInInspector] public float avgYSpeed = 0;
    [HideInInspector] public float avgXSpeed = 0;
    [HideInInspector] public float avgQESpeed = 0;
    [HideInInspector] public float speedIterations = 0;

    private float swimAudioTimer = 0;
    private float swimSoundInterval = 0;
    private bool stuckOnLand = false;
    public float landTimer = 0f;

    public bool xPress1, xPress2, yPress1, yPress2, zPress = false;

    public bool OldControls = false;

    private TrickHandler TH;

    public bool waterEntry = false;
    public bool waterExit = false;
    void Awake()
    {
        boneZone = GetComponent<BoneController>();
        ui = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
        swimSoundInterval = Random.Range(0.6f, 0.8f);
        camFollow = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraFollow>();
        currentState = State.Above;
        anim = transform.GetChild(0).GetComponent<Animator>();
        audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioController>();
        TH = gameObject.GetComponent<TrickHandler>();
        transform.position = new Vector3(0, 500, 0);
        transform.rotation = Quaternion.Euler(Random.Range(70, 120), 0, 0);
    }
    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OldControls = !OldControls;
        }
        if (OldControls)
        {
            xAxis = (int) Input.GetAxisRaw("Horizontal");
            zAxis = (int) Input.GetAxisRaw("Vertical");
            yAxis = Input.GetKey(KeyCode.Space) ? -1 : Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
            qeAxis = Input.GetKey(KeyCode.E) ? -1 : Input.GetKey(KeyCode.Q) ? 1 : 0;
        }
        else
        {
            //control input
            xAxis = (int) Input.GetAxisRaw("Horizontal");
            yAxis = (int) Input.GetAxisRaw("Vertical");
            zAxis = Input.GetKey(KeyCode.LeftShift) ? 0 : 1;
            qeAxis = Input.GetKey(KeyCode.E) ? -1 : Input.GetKey(KeyCode.Q) ? 1 : 0;
        }

        //scale speed through input
        currentXSpeed = SpeedScale(xAxis, currentXSpeed, maxRotationSpeed, rotationStep);
        currentYSpeed = SpeedScale(yAxis, currentYSpeed, maxRotationSpeed, rotationStep);
        currentZSpeed = SpeedScale(zAxis, currentZSpeed, maxSpeed, speedStep);
        currentQESpeed = SpeedScale(qeAxis, currentQESpeed, maxRotationSpeed, rotationStep);
        if (!inWaterBefore)
        {
            rb.angularVelocity = (rb.angularVelocity + new Vector3(0, Random.Range(0, 10), 0)).normalized * 10;
        }
        if (!ui.startScreen)
        {
            boneZone.buttonPressed = currentState == State.Above
                        && (Input.GetKeyDown(KeyCode.Q) 
                            || Input.GetKeyDown(KeyCode.E) 
                            || Input.GetKey(KeyCode.LeftShift)
                            || Input.GetKey(KeyCode.Space)
                            || Input.GetAxisRaw("Horizontal") == 0
                            || Input.GetAxisRaw("Vertical") == 0);
            if (boneZone.animationReady) anim.enabled = !boneZone.buttonPressed;

            switch (currentState)
            {
                case State.Submerged:
                    if (zAxis != 0) // audio timer for swimming
                    {
                        if (swimAudioTimer > swimSoundInterval)
                        {
                            audio.PlaySFX(4, true);
                            swimSoundInterval = Random.Range(0.5f, 0.8f);
                            swimAudioTimer = 0;
                        }
                        swimAudioTimer += Time.deltaTime;
                    }

                    if (gravity < 0.1 && gravity > -0.1)
                    {
                        gravity = 0;
                    }
                    else if (gravity > 0)
                    {
                        gravity -= Time.deltaTime * gravityScale;
                    }

                    if (qeAxis == 0 && yAxis == 0) //autocorrect qe and y axes
                    {
                        Quaternion posToRotate = new Quaternion();
                        posToRotate.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                        transform.rotation = Quaternion.Lerp(transform.rotation, posToRotate, smoothTime);
                    }
                    if (ui.tutorialActive && ui.currentStage == 1 && zAxis == 0)
                    {
                        ui.TaskCompleted(1);
                    }
                    if (ui.tutorialActive && ui.currentStage == 2)
                    {
                        if (xAxis == 1 && !xPress1)
                        {
                            xPress1 = true;
                        }
                        if (xAxis == -1 && !xPress2)
                        {
                            xPress2 = true;
                        }
                        if (xPress1 && xPress2)
                        {
                            ui.TaskCompleted(2);
                        }
                    }
                    if (ui.tutorialActive && ui.currentStage == 3)
                    {
                        if (yAxis == 1 && !yPress1)
                        {
                            yPress1 = true;
                        }
                        if (yAxis == -1 && !yPress2)
                        {
                            yPress2 = true;
                        }
                        if (yPress1 && yPress2)
                        {
                            ui.TaskCompleted(3);
                        }
                    }
       

                    break;
                case State.Above:
                    Vector3 universalBoneRot = new Vector3(yAxis * 20f, -qeAxis * 20f, xAxis * 30f);
                    
                    boneZone.midRotation = universalBoneRot;
                    boneZone.tailRotation = universalBoneRot;

                    swimAudioTimer = 0;

                    ZRotationSpeed = SpeedScale(yAxis, ZRotationSpeed, maxRotationSpeed, rotationStep);
                    gravity += (Time.deltaTime * gravityScale);

                    if (stuckOnLand)
                    {
                        landTimer += Time.deltaTime;
                        if (landTimer >= 5) respawnButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        landTimer = 0f;
                        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rb.velocity, Vector3.up), 0.1f);
                    }
                    break;
            }

            if (Mathf.Abs(zAxis) == 1)
            {
                anim.SetBool("Swimming", true);
            }
            else
            {
                anim.SetBool("Swimming", false);
            }
        }
    }


    void FixedUpdate()
    {
        if (!ui.startScreen)
        {
            //angular velocity decay
            rb.angularVelocity *= 0.95f;
            float AVMag = rb.angularVelocity.magnitude;
            if (AVMag < 0.05f && AVMag > -0.05f) rb.angularVelocity = Vector3.zero;


            if (currentState == State.Submerged)
            {
                rb.velocity = transform.forward * currentZSpeed + Vector3.down * gravity;
                transform.Rotate(new Vector3(currentYSpeed, currentXSpeed, currentQESpeed), Space.Self);
            }
            else if (currentState == State.Above)
            {
                transform.Rotate(new Vector3(ZRotationSpeed * 2, currentXSpeed * 2, currentQESpeed * 2), Space.Self);
                //UpdateAVGSpeeds();
                rb.velocity += Vector3.down * gravity;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            splash.Play();
            bubbles.Play();
            waterEntry = true;
            waterExit = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Water")
        {
            stuckOnLand = false;
            if (currentState == State.Above)
            {
                camFollow.EnteredWater();
                audio.PlaySFX(5, true);
                currentState = State.Submerged;
                if (!inWaterBefore)
                {
                    inWaterBefore = true;
                    ui.FirstTimeInWater();
                }
                
            }
            if (!anim.GetBool("InWater"))
            {
                anim.SetBool("InWater", true);
            }
        }
        else if (other.tag == "Railgun")
        {
            //Debug.Log("Nyyyyyyyooooooom");
            rb.AddForce(other.transform.forward * 475f);
        }
        else if(other.tag == "Geyser")
        {
            Vector3 slowVel = rb.velocity;
            slowVel.x *= 0.5f;
            slowVel.z *= 0.5f;
            rb.velocity = slowVel;
            rb.AddForce(other.transform.forward * 750f);
        }
        else if (other.tag == "JetStream")
        {
            Vector3 attraction = Vector3.zero - rb.position;
            attraction.y = 0f;
            rb.AddForce(attraction.normalized * 10f);
        }
    }
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Terrain" && currentState != State.Submerged)
        {
            stuckOnLand = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Terrain" && currentState != State.Submerged && rb.velocity.magnitude > 20)
        {
            audio.PlaySFX(8, false);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            if (currentState == State.Submerged)
            {
                audio.PlaySFX(1, true);
                anim.SetBool("InWater", false);
                rb.AddForce(transform.forward * jumpForce * currentZSpeed, ForceMode.Impulse);
                currentState = State.Above;
                waterEntry = false;
                waterExit = true;

                bubbles.Stop();
            }
        }
        if (other.tag == "Railgun")
        {
            audio.PlaySFX(7, false);
        }
        if (other.tag == "Hoop")
        {
            TH.AddScoreGeneral(2, true, false, false);
            audio.PlaySFX(3, false);
        }
    }

    public float SpeedScale(int axis, float actualSpeed, float targetSpeed, float step)
    {
        switch (axis)
        {
            case (-1):
                if (actualSpeed > -targetSpeed)
                {
                    actualSpeed -= targetSpeed * step;
                }
                break;
            case (0):
                if (actualSpeed < step && actualSpeed > -step)
                {
                    actualSpeed = 0;
                }
                else if (actualSpeed > 0)
                {
                    actualSpeed -= targetSpeed * step;
                }
                else if (actualSpeed < 0)
                {
                    actualSpeed += targetSpeed * step;
                }
                break;
            case (1):
                if (actualSpeed < targetSpeed)
                {
                    actualSpeed += targetSpeed * step;
                }
                break;
        }
        return actualSpeed;
    }

    public string StateCheck()
    {
        return currentState.ToString();
    }
}
