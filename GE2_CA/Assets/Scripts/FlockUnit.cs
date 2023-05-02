using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] private float FOVAngle; // Field of view angle for the flocking unit
    [SerializeField] private float smoothDamp; // Smoothing factor used for movement
    [SerializeField] private LayerMask obstacleMask; // Layer mask used to detect obstacles
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles; // Array of directions to check for obstacles

    // Lists to store different types of neighboring flocking units
    private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    private List<FlockUnit> avoidanceNeighbours = new List<FlockUnit>();
    private List<FlockUnit> aligementNeighbours = new List<FlockUnit>();

    // Reference to the flock this unit belongs to
    private Flock assignedFlock;

    // Current velocity of the flocking unit
    private Vector3 currentVelocity;

    // Vector used to avoid obstacles
    private Vector3 currentObstacleAvoidanceVector;

    // Speed of the flocking unit
    private float speed;

    // Public getter and setter for the transform component of the flocking unit
    public Transform myTransform { get; set; }

    private void Awake()
    {
        // Assign the transform component to the public getter and setter
        myTransform = transform;
    }

    public void AssignFlock(Flock flock)
    {
        // Assign the flock this unit belongs to
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        // Initialize the speed of the flocking unit
        this.speed = speed;
    }

    public void MoveUnit()
    {
        // Find neighboring units for cohesion, avoidance, and alignment
        FindNeighbours();

        // Calculate the speed of the flocking unit
        CalculateSpeed();

        // Calculate the vectors for cohesion, avoidance, alignment, bounds, and obstacle avoidance
        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

        // Combine all the vectors into a final movement vector
        var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector;

        // Smooth the movement vector
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);

        // Scale the movement vector by the speed of the flocking unit
        moveVector = moveVector.normalized * speed;

        // If the movement vector is zero, use the forward vector of the flocking unit instead
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        // Rotate the flocking unit towards the movement vector
        myTransform.forward = moveVector;

        // Move the flocking unit in the direction of the movement vector
        myTransform.position += moveVector * Time.deltaTime;
    }

    private void FindNeighbours() // Populates lists of neighbouring units for the current unit based on their distance from the current unit and the distances specified for the flock's behaviours.
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        aligementNeighbours.Clear();
        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.myTransform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.aligementDistance * assignedFlock.aligementDistance)
                {
                    aligementNeighbours.Add(currentUnit);
                }
            }
        }
    }

    private void CalculateSpeed() // Calculates the cohesion vector for the current unit, which points towards the average position of neighbouring units within the current unit's field of view (FOV)
    {
        if (cohesionNeighbours.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        speed /= cohesionNeighbours.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }

    private Vector3 CalculateCohesionVector() // Calculates the alignment vector for the current unit, which points towards the average forward direction of neighbouring units within the current unit's FOV
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return cohesionVector;
    }

    private Vector3 CalculateAligementVector() // Calculates the avoidance vector for the current unit, which points away from neighbouring units within the current unit's FOV
    {
        var aligementVector = myTransform.forward;
        if (aligementNeighbours.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < aligementNeighbours.Count; i++)
        {
            if (IsInFOV(aligementNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                aligementVector += aligementNeighbours[i].myTransform.forward;
            }
        }

        aligementVector /= neighboursInFOV;
        aligementVector = aligementVector.normalized;
        return aligementVector;
    }

    private Vector3 CalculateAvoidanceVector() // Calculates a vector to avoid collisions with other boids by checking the neighbouring boids in the field of view (FOV) of the current boid
    {
        var avoidanceVector = Vector3.zero;
        if (aligementNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    private Vector3 CalculateBoundsVector() // Calculates a vector to keep the boids within the bounds of the assigned flock
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector() // Calculates a vector to avoid obstacles in the environment
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }
        return obstacleVector;
    }

    private Vector3 FindBestDirectionToAvoidObstacle() // Calculates the direction that avoids the obstacle by checking a set of predefined directions relative to the boid's forward direction
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {

            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.obstacleDistance, obstacleMask))
            {

                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }

    private bool IsInFOV(Vector3 position) // Check if a given position is within the FOV of the current boid
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}