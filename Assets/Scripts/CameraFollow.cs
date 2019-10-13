using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target; // target object for camera to follow
    [SerializeField] float smoothSpeed = 10f; // speed of camera movement
    [SerializeField] Vector3 offset; // camera position in relation to the target

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // assign the desired position of the camera in relation to the player
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // interpolate the camera's movement vector
        transform.position = smoothedPosition; // update the camera's position to follow the movement vector

        transform.LookAt(target); // make sure the target is always in view of the camera
    }
}
