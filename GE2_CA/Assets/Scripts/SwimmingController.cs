using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingController : MonoBehaviour
{
    public float movementSpeed = 5.0f; // The movement speed of the Player
    public float mouseSensitivity = 1.5f; // The sensitivity of the mouse movement
    public float maxVerticalAngle = 60.0f; // The maximum vertical angle the camera can look
    public float sinkSpeed = 0.75f; // The speed at which the Player sinks down when not moving up or down
    public float smoothTime = 2f; // The smooth time for camera movement

    private Vector2 currentRotation; // The current rotation of the camera
    private Vector3 currentVelocity; // The current velocity of the camera

    void Start()
    {
        // Locks the cursor to the center of the screen, essentially hiding it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // The following block of code moves the camera based on input
        float moveForwardBackward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveLeftRight = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float moveUpDown = 0;

        // Disable sinking if player is moving forward/backward or strafing left/right
        if (moveForwardBackward != 0 || moveLeftRight != 0)
        {
            sinkSpeed = 0;
        }
        else
        {
            sinkSpeed = 0.5f;
        }

        // Move the camera up or down based on input
        if (Input.GetKey(KeyCode.Space))
        {
            moveUpDown = movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            moveUpDown = -movementSpeed * Time.deltaTime;
        }
        else
        {
            moveUpDown = -sinkSpeed * Time.deltaTime;
        }

        transform.Translate(moveLeftRight, moveUpDown, moveForwardBackward);

        // Gradually slow down the Player's movement when the Player releases input keys
        Vector3 targetPosition = transform.position + new Vector3(moveLeftRight, moveUpDown, moveForwardBackward);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // The following block of code rotates the camera based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentRotation.x += mouseX;
        currentRotation.y -= mouseY;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxVerticalAngle, maxVerticalAngle);
        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
