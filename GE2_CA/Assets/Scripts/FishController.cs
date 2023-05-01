using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public float swimSpeed = 2f; // The speed at which the fish swims forward
    public float turnSpeed = 90f; // The maximum angle the fish can turn per frame
    public float maxTurnSpeed = 80f; // The maximum turn speed of the fish

    private Quaternion targetRotation;

    void Start()
    {
        // Initialize the target rotation to the current rotation
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Move the fish forward in its local space
        transform.Translate(Vector3.forward * swimSpeed * Time.deltaTime, Space.Self);

        // Generate a random angle to turn the fish
        float turnAngle = Random.Range(-maxTurnSpeed, maxTurnSpeed);

        // Get the current forward direction of the fish
        Vector3 currentForward = transform.forward;

        // Calculate the new forward direction of the fish after the turn
        Vector3 newForward = Quaternion.AngleAxis(turnAngle, Vector3.up) * currentForward;

        // Calculate the target rotation towards the new forward direction
        targetRotation = Quaternion.LookRotation(newForward, transform.up);

        // Smoothly rotate the fish towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime / Quaternion.Angle(transform.rotation, targetRotation));
    }
}