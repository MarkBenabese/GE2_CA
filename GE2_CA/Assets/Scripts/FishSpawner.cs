using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab; // Reference to the fish prefab
    public GameObject spawnArea; // Reference to the spawn area game object

    public int numberOfFish = 10; // The number of fish to spawn
    public float spawnDelay = 0.5f; // The delay between spawning fish
    public float moveSpeed = 2f; // The speed at which the fish moves

    void Start()
    {
        // Loop to spawn the specified number of fish
        for (int i = 0; i < numberOfFish; i++)
        {
            // Wait for the specified delay before spawning the next fish
            StartCoroutine(SpawnFishWithDelay(i * spawnDelay));
        }
    }

    IEnumerator SpawnFishWithDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Get a random position within the spawn area
        Vector3 randomPos = GetRandomPosition();

        // Instantiate the fish prefab at the random position
        GameObject fish = Instantiate(fishPrefab, randomPos, Quaternion.identity);

        // Get a random angle to face the fish in the X and Z axis
        float randomAngle = Random.Range(0f, 360f);

        // Calculate the forward vector of the fish based on its rotation
        Vector3 fishForward = fish.transform.forward;

        // If the fish is facing negative X-axis, flip its forward vector
        if (fish.transform.forward.x < 0f)
        {
            fishForward = -fishForward;
        }

        // Ignore the Y-axis of the forward vector
        fishForward.y = 0f;

        // If the forward vector is zero, default to the world X-axis
        if (fishForward == Vector3.zero)
        {
            fishForward = Vector3.right;
        }

        // Rotate the fish to face the random angle in the X and Z axis
        fish.transform.rotation = Quaternion.LookRotation(fishForward) * Quaternion.Euler(0f, randomAngle, 0f);

        // Set the fish's parent to this game object so that it doesn't clutter the hierarchy
        fish.transform.SetParent(transform);

        // Get the fish's rigidbody component
        Rigidbody fishRigidbody = fish.GetComponent<Rigidbody>();

        // Set the fish's velocity to move in its local negative X-axis
        fishRigidbody.velocity = -fish.transform.right * moveSpeed;
    }

    Vector3 GetRandomPosition()
    {
        // Get the bounds of the spawn area
        Bounds bounds = spawnArea.GetComponent<Collider>().bounds;

        // Get a random point within the bounds of the spawn area
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        // Return the random position as a Vector3
        return new Vector3(randomX, randomY, randomZ);
    }
}