using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class CarController : MonoBehaviour
{
    public InputManager input;
    public List<WheelCollider> accelerationWheels;
    public List<WheelCollider> steeringWheels;
    public float strengthCoefficient = 20000f;
    public float maxTurnAngle = 20f;
    public Transform centerMass;

    [SerializeField] float levelLoadDelay = 2f; // delay time before level load on death and success

    // particle systems
    [SerializeField] ParticleSystem exhaustParticles; // particle system for car exhaust
    [SerializeField] ParticleSystem explosionParticles; // particle system for death explosion
    [SerializeField] ParticleSystem successParticles; // particle system for level completion

    // audio clips
    [SerializeField] AudioClip engine; // audio clip for engine acceleration
    [SerializeField] AudioClip death; // audio clip for death explosion
    [SerializeField] AudioClip success; // audio clip for level completion

    // current world state
    bool isTransitioning = false; // is a level transition happening?
    bool collisionsDisabled = false; // are collisions disabled?

    public Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputManager>();
        rigidBody.GetComponent<Rigidbody>();
        if (centerMass)
        {
            rigidBody.centerOfMass = centerMass.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(WheelCollider wheel in accelerationWheels)
        {
            wheel.motorTorque = strengthCoefficient * Time.deltaTime * input.accelerate;
        }
        foreach(WheelCollider wheel in steeringWheels)
        {
            wheel.steerAngle = maxTurnAngle * input.steer;
        }
    }
}
