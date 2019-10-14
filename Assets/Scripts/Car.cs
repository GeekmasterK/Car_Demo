using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] float maxSpeed = 6f; // maximum speed
    [SerializeField] float accelTime = 2.5f; // time to reach 0 to max speed
    [SerializeField] float decelTime = 6f; // time to reach max speed to 0
    [SerializeField] float brakeTime = 1f; // time to reach max speed to 0 when braking

    float accelRatePerSec; // rate of acceleration per second
    float forwardVelocity; // velocity of the car on the z-axis
    float decelRatePerSec; // rate of deceleration per second
    float brakeRatePerSec; // rate of deceleration per second when braking 

    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        accelRatePerSec = maxSpeed / accelTime;
        decelRatePerSec = -maxSpeed / decelTime;
        brakeRatePerSec = -maxSpeed / brakeTime;
        forwardVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RespondToAccelInput();        
        // TODO add left/right steering
    }

    /*
     * accelerate when pressing the spacebar (applying the gas)
     */
    void RespondToAccelInput()
    {
        if (Input.GetKey(KeyCode.W)) // can accelerate while turning
        {
            Accelerate(accelRatePerSec);
        }
        else if(Input.GetKey(KeyCode.Z))
        {
            Accelerate(brakeRatePerSec);
        }
        else
        {
            Accelerate(decelRatePerSec);
        }
    }

    void Accelerate(float accel)
    {
        forwardVelocity += accel * Time.deltaTime; // increase forward velocity by accelRatePerSec once per frame
        forwardVelocity = Mathf.Clamp(forwardVelocity, 0f, maxSpeed); // prevent forward velocity from exceeding maximum speed
        rigidBody.velocity = Vector3.forward * forwardVelocity; // accelerate forward
    }
}
