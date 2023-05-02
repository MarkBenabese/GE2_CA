using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;  // Prefab of the flock unit object
    [SerializeField] private int flockSize;  // Number of flock units to spawn
    [SerializeField] private Vector3 spawnBounds;  // Bounds within which to spawn flock units

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float _minSpeed;  // Minimum speed of flock units
    public float minSpeed { get { return _minSpeed; } }  // Property to get the minimum speed
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed;  // Maximum speed of flock units
    public float maxSpeed { get { return _maxSpeed; } }  // Property to get the maximum speed


    [Header("Detection Distances")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;  // Distance for cohesion behavior
    public float cohesionDistance { get { return _cohesionDistance; } }  // Property to get the cohesion distance

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;  // Distance for avoidance behavior
    public float avoidanceDistance { get { return _avoidanceDistance; } }  // Property to get the avoidance distance

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance;  // Distance for alignment behavior
    public float aligementDistance { get { return _aligementDistance; } }  // Property to get the alignment distance

    [Range(0, 10)]
    [SerializeField] private float _obstacleDistance;  // Distance for obstacle avoidance behavior
    public float obstacleDistance { get { return _obstacleDistance; } }  // Property to get the obstacle avoidance distance

    [Range(0, 100)]
    [SerializeField] private float _boundsDistance;  // Distance for staying within the bounds
    public float boundsDistance { get { return _boundsDistance; } }  // Property to get the bounds distance


    [Header("Behaviour Weights")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;  // Weight of cohesion behavior
    public float cohesionWeight { get { return _cohesionWeight; } }  // Property to get the cohesion weight

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;  // Weight of avoidance behavior
    public float avoidanceWeight { get { return _avoidanceWeight; } }  // Property to get the avoidance weight

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight;  // Weight of alignment behavior
    public float aligementWeight { get { return _aligementWeight; } }  // Property to get the alignment weight

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;  // Weight of staying within the bounds
    public float boundsWeight { get { return _boundsWeight; } }  // Property to get the bounds weight

    [Range(0, 100)]
    [SerializeField] private float _obstacleWeight;  // Weight of obstacle avoidance behavior
    public float obstacleWeight { get { return _obstacleWeight; } }  // Property to get the obstacle avoidance weight

    public FlockUnit[] allUnits { get; set; }  // Array of all the spawned flock units

    private void Start()
    {
        GenerateUnits(); // Calls a function to create and spawn flocking units
    }

    private void Update()
    {
        // For each unit in the flock, move it
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    // Generates flocking units with randomized positions, rotations, and speeds
    private void GenerateUnits()
    {
        // Create an array to store all the flocking units
        allUnits = new FlockUnit[flockSize];

        // For each unit we want to create...
        for (int i = 0; i < flockSize; i++)
        {
            // Generate a random vector within a given bounds
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);

            // Calculate a spawn position by adding the random vector to the parent transform's position
            var spawnPosition = transform.position + randomVector;

            // Generate a random rotation
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

            // Instantiate a new flocking unit at the spawn position and rotation
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);

            // Assign this flock as the flocking unit's flock
            allUnits[i].AssignFlock(this);

            // Initialize the flocking unit's speed with a randomized value between minSpeed and maxSpeed
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }
}
