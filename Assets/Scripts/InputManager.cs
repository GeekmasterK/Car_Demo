using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float accelerate; // acceleration input variable
    public float steer; // steering input variable

    // Update is called once per frame
    void Update()
    {
        SetInputValues();
    }

    /*
     * assign values to the accelerate and steer input variables
     */
    void SetInputValues()
    {
        accelerate = Input.GetAxisRaw("Vertical"); // accelerate forward or backward
        steer = Input.GetAxisRaw("Horizontal"); // steer left or right
    }
}
