using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    void Update()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) <= 0.5f)
        {
            // Set target to current position of original target
            target.position = GameObject.FindWithTag("Player").transform.position;
        }
    }
}