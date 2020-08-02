using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target; // target object for camera to follow
    [SerializeField] float smoothSpeed = 10f; // speed of camera movement
    [SerializeField] Vector3 offset; // camera position in relation to the target
    
    void FixedUpdate()
    {
        CameraMove();
        CameraRotate();
    }

    /*
     * move the camera behind the player
     */
    void CameraMove()
    {
        Vector3 desiredPosition = target.position + offset; // assign the desired position of the camera in relation to the player
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // interpolate the camera's movement vector
        transform.position = smoothedPosition; // update the camera's position to follow the movement vector

        transform.LookAt(target); // make sure the target is always in view of the camera
    }

    /*
     * rotate the camera to face the player
     */
    void CameraRotate()
    {
        Vector3 desiredRotation = target.position + (-target.forward * offset.magnitude); // assign the desired rotation of the camera in relation to the player
        Vector3 smoothedRotation = Vector3.Lerp(transform.position, desiredRotation, smoothSpeed * Time.deltaTime); // interpolate the camera's rotation vector
        transform.position = smoothedRotation; // update the camera's rotation to follow the rotation vector

        transform.LookAt(target); // make sure the target is always in view of the camera
    }
}
