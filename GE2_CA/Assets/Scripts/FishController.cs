using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public float moveSpeed = 2f; // The speed at which the fish moves
    public string aquariumTag = "Aquarium"; // The tag of the objects to detect collisions with

    private Rigidbody rb; // The fish's Rigidbody component

    void Start()
    {
        // Get the fish's Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Set the fish's velocity to move in its local negative X-axis
        rb.velocity = -transform.right * moveSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision was with an object tagged "Aquarium"
        if (collision.gameObject.CompareTag(aquariumTag))
        {
            // Change the fish's direction to a random direction
            rb.velocity = Random.insideUnitSphere * moveSpeed;
        }
    }
}