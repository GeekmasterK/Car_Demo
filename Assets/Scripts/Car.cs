using UnityEngine;

public class Car : MonoBehaviour
{
    // car attributes
    [SerializeField] float maxSpeed = 6f; // maximum speed
    [SerializeField] float accelTime = 2.5f; // time to reach 0 to max speed
    [SerializeField] float decelTime = 6f; // time to reach max speed to 0
    [SerializeField] float brakeTime = 1f; // time to reach max speed to 0 when braking
    [SerializeField] float turnAnglePerSec = 90f; // steering angle adjustment per second
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

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        accelRatePerSec = maxSpeed / accelTime;
        decelRatePerSec = -maxSpeed / decelTime;
        //brakeRatePerSec = -maxSpeed / brakeTime;
        forwardVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RespondToAccelInput();
        RespondToTurnInput();
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
        }
        else if(accelReverse)
        {
            rigidBody.velocity = -transform.forward * forwardVelocity; // accelerate backward
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
