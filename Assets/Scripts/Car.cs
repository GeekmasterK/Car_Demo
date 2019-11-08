using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    // car attributes
    [SerializeField] float maxSpeed = 6f; // maximum speed
    [SerializeField] float accelTime = 2.5f; // time to reach 0 to max speed
    [SerializeField] float decelTime = 6f; // time to reach max speed to 0
    //[SerializeField] float brakeTime = 1f; // time to reach max speed to 0 when braking
    [SerializeField] float turnAnglePerSec = 90f; // steering angle adjustment per second
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip engine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    float leftTurn = -1f; // modifier to calculate left turn
    float rightTurn = 1f; // modifier to calculate right turn

    // attributes calculated at runtime
    float accelRatePerSec; // rate of acceleration per second
    float decelRatePerSec; // rate of deceleration per second
    // float brakeRatePerSec; // rate of deceleration per second when braking 

    // current car state
    float forwardVelocity; // velocity of the car on the z-axis
    float currentRotation; // current car rotation
    bool accelForward;
    bool accelReverse;
    //bool decel;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        accelRatePerSec = maxSpeed / accelTime;
        decelRatePerSec = -maxSpeed / decelTime;
        //brakeRatePerSec = -maxSpeed / brakeTime;
        forwardVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTransitioning)
        {
            RespondToAccelInput();
            RespondToTurnInput();
        }

        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFirstLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(isTransitioning || collisionsDisabled)
        {
            return;
        }
        switch(collision.gameObject.tag)
        {
            case "Ground":
                // do nothing
                break;
            case "Goal":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        rigidBody.freezeRotation = false;
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    /*
     * accelerate when pressing the spacebar (applying the gas)
     */
    void RespondToAccelInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0f) // can accelerate while turning
        {
            accelForward = true;
            accelReverse = false;
            Accelerate(accelRatePerSec);
        }
        else if(Input.GetAxisRaw("Vertical") < 0f)
        {
            accelForward = false;
            accelReverse = true;
            Accelerate(accelRatePerSec);
        }
        /*else if (Input.GetKey(KeyCode.Z))
        {
            Accelerate(brakeRatePerSec);
        }*/
        else
        {
            accelForward = false;
            accelReverse = false;
            Accelerate(decelRatePerSec);
        }
    }

    void RespondToTurnInput()
    {
        if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            Turn(leftTurn);
        }
        if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            Turn(rightTurn);
        }
    }

    void Accelerate(float accel)
    {
        forwardVelocity += accel * Time.deltaTime; // increase forward velocity by accelRatePerSec once per frame
        forwardVelocity = Mathf.Clamp(forwardVelocity, 0f, maxSpeed); // prevent forward velocity from exceeding maximum speed
        if(accelForward)
        {
            rigidBody.velocity = transform.forward * forwardVelocity; // accelerate forward
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(engine);
            }
        }
        else if(accelReverse)
        {
            rigidBody.velocity = -transform.forward * forwardVelocity; // accelerate backward
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(engine);
            }
        }
    }

    void Turn(float direction)
    {
        currentRotation = turnAnglePerSec * Time.deltaTime * direction;
        
        // if moving forward, turn car
        if (forwardVelocity > 0)
        {
            rigidBody.rotation = Quaternion.Euler(rigidBody.rotation.eulerAngles + new Vector3(0f, currentRotation, 0f));
        }
        
        // if no acceleration input, decelerate
        if(!accelForward && !accelReverse)
        {
            Accelerate(decelRatePerSec);
        }

        // reset for next frame
        accelForward = false;
        accelReverse = false;
        currentRotation = 0f;
    }
}
