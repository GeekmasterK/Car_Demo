using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car1 : MonoBehaviour
{
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    public WheelCollider frontLeftWheel, frontRightWheel;
    public WheelCollider backLeftWheel, backRightWheel;
    public Transform frontLeftTransform, frontRightTransform;
    public Transform backLeftTransform, backRightTransform;
    public float maxSteerAngle = 30f;
    public float accelForce = 50f;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateAllWheelPoses();
    }

    public void GetInput()
    {
        m_horizontalInput = Input.GetAxisRaw("Horizontal");
        m_verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Steer()
    {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontRightWheel.steerAngle = m_steeringAngle;
        frontLeftWheel.steerAngle = m_steeringAngle;
    }

    private void Accelerate()
    {
        frontRightWheel.motorTorque = m_verticalInput * accelForce;
        frontLeftWheel.motorTorque = m_verticalInput * accelForce;
    }

    private void UpdateAllWheelPoses()
    {
        UpdateSingleWheelPose(frontRightWheel, frontRightTransform);
        UpdateSingleWheelPose(frontLeftWheel, frontLeftTransform);
        UpdateSingleWheelPose(backRightWheel, backRightTransform);
        UpdateSingleWheelPose(backLeftWheel, backLeftTransform);
    }

    private void UpdateSingleWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;

        collider.GetWorldPose(out pos, out quat);
    }
}
