using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    // car attributes
    [SerializeField] float maxSpeed = 6f; // maximum speed
    [SerializeField] float accelTime = 2.5f; // time to reach 0 to max speed
    [SerializeField] float decelTime = 6f; // time to reach max speed to 0
    [SerializeField] float turnAnglePerSec = 90f; // steering angle adjustment per second
    [SerializeField] float levelLoadDelay = 2f; // delay time before level load on death and success

    // particle systems
    [SerializeField] ParticleSystem exhaustParticles; // particle system for car exhaust
    [SerializeField] ParticleSystem explosionParticles; // particle system for death explosion
    [SerializeField] ParticleSystem successParticles; // particle system for level completion

    // audio clips
    [SerializeField] AudioClip engine; // audio clip for engine acceleration
    [SerializeField] AudioClip death; // audio clip for death explosion
    [SerializeField] AudioClip success; // audio clip for level completion

    // modifiers
    float leftTurn = -1f; // directional modifier to calculate left turn
    float rightTurn = 1f; // directional modifier to calculate right turn

    // attributes calculated at runtime
    float accelRatePerSec; // rate of acceleration per second
    float decelRatePerSec; // rate of deceleration per second

    // current car state
    float forwardVelocity; // velocity of the car on the z-axis
    float currentRotation; // current car rotation
    bool accelForward; // is the car accelerating forward?
    bool accelReverse; // is the car accelerating in reverse?

    Rigidbody rigidBody;
    AudioSource audioSource;

    // current world state
    bool isTransitioning = false; // is a level transition happening?
    bool collisionsDisabled = false; // are collisions disabled?

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        accelRatePerSec = maxSpeed / accelTime;
        decelRatePerSec = -maxSpeed / decelTime;
        forwardVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // if a level transition is not happening...
        if(!isTransitioning)
        {
            RespondToAccelInput(); // enable forward/reverse accelreation input
            RespondToTurnInput(); // enable left/right turn input
        }
        // if this is a dev build...
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys(); // enable debug keys
        }
    }

    /*
     * get debug key input for testing a dev build
     */
    void RespondToDebugKeys()
    {
        // if the L key is pressed...
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadFirstLevel(); // reset the game to the beginning
        }
        // otherwise, if the C key is pressed...
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // disable collisions
        }
    }

    /*
     * determine car behavior on collision with another object
     */
    void OnCollisionEnter(Collision collision)
    {
        // if a level transition is happening and/or collisions are disabled...
        if(isTransitioning || collisionsDisabled)
        {
            return; // exit this function
        }
        // car behavior based on the tag of the object collided with
        switch(collision.gameObject.tag)
        {
            case "Ground":
                // do nothing
                break;
            case "Goal":
                StartSuccessSequence(); // trigger win condition
                break;
            default:
                StartDeathSequence(); // trigger death condition
                break;
        }
    }

    /*
     * notify the player on reaching the win condition
     */
    void StartSuccessSequence()
    {
        isTransitioning = true; // a level transition is happening
        audioSource.Stop(); // stop playing audio
        audioSource.PlayOneShot(success); // play the success sound effect
        successParticles.Play(); // trigger the success particle effect
        Invoke("LoadFirstLevel", levelLoadDelay); // reset the level
    }

    /*
     * explode car on player death
     */
    void StartDeathSequence()
    {
        isTransitioning = true; // a level transition is happening
        audioSource.Stop(); // stop playing audio
        audioSource.PlayOneShot(death); // play the death sound effect
        explosionParticles.Play(); // trigger the explosion particle effect
        rigidBody.freezeRotation = false; // unfreeze rotation physics
        Invoke("LoadFirstLevel", levelLoadDelay); // reset the level
    }

    /*
     * reset the game
     */
    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0); // load the first scene in the build
    }

    /*
     * accelerate forward or backward
     */
    void RespondToAccelInput()
    {
        // if pressing the forward input...
        if (Input.GetAxisRaw("Vertical") > 0f)
        {
            accelForward = true; // accelerating forward
            accelReverse = false; // not accelerating in reverse
            Accelerate(accelRatePerSec); // accelerate at the designated rate per second
        }
        // otherwise, if pressing the backward input...
        else if(Input.GetAxisRaw("Vertical") < 0f)
        {
            accelForward = false; // not accelerating forward
            accelReverse = true; // accelerating in reverse
            Accelerate(accelRatePerSec); // accelerate at the designated rate per second
        }
        // otherwise...
        else
        {
            accelForward = false; // not accelerating forward
            accelReverse = false; // not accelerating backward
            Accelerate(decelRatePerSec); // decelerate at the designated rate per second
            StopAccelerating(); // stop the acceleration audio and exhaust particles
        }
    }

    /*
     * 
     */
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
        exhaustParticles.Play();
    }

    void StopAccelerating()
    {
        audioSource.Stop();
        exhaustParticles.Stop();
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
