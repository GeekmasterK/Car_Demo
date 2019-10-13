using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] float maxSpeed = 6f; // maximum speed
    [SerializeField] float accelTime = 2.5f; // time to reach 0 to max speed
    float accelRatePerSec; // rate of acceleration per second
    float forwardVelocity; // velocity of the car on the z-axis

    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        accelRatePerSec = maxSpeed / accelTime;
        forwardVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RespondToAccelInput();
        // TODO add braking/deceleration
        // TODO add left/right steering
    }

    /*
     * accelerate when pressing the spacebar (applying the gas)
     */
    private void RespondToAccelInput()
    {
        if (Input.GetKey(KeyCode.W)) // can accelerate while turning
        {
            ApplyAcceleration();
        }
    }

    /*
     * move the car forward by acceleration
     */
    private void ApplyAcceleration()
    {
        forwardVelocity += accelRatePerSec * Time.deltaTime; // increase forward velocity by accelRatePerSec once per frame
        forwardVelocity = Mathf.Min(forwardVelocity, maxSpeed); // prevent forward velocity from exceeding maximum speed
        rigidBody.velocity = Vector3.forward * forwardVelocity; // accelerate forward
    }
}
