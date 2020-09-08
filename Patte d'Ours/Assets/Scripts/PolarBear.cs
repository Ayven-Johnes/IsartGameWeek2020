using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarBear : MonoBehaviour
{
    public Transform targetWaypoint = null;
    public int currentIndexWaypoint = 0;

    public float minDistance = 0.1f;

    public float movementSpeed = 2.0f;

    public float rotationSpeed = 2.0f;

    public int chanceToDoAction = 80;
    public bool actionLauch = false;

    public bool needNewWaypoint = false;

    // Update is called once per frame
    void Update()
    {
        if (actionLauch)
        {
            CheckActionFinish();
        }
        else if (!needNewWaypoint)
        {
            UpdateTransform();

            float distance = Vector3.Distance(transform.position, targetWaypoint.position);
            CheckDistanceToWaypoint(distance);
        }
    }

    void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance  <= minDistance)
        {
            needNewWaypoint = true;

            /*if (Random.Range(1, 100) <= chanceToDoAction)
            {
                // launch Action
                actionLauch = true;
            }*/
        }
    }

    void CheckActionFinish()
    {

    }

    void UpdateTransform()
    {
        float movementStep = movementSpeed * Time.deltaTime;

        float rotationStep = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, movementStep);
    }

    public void UpdateWaypoint(Transform waypoint, int indexWaypoint)
    {
        currentIndexWaypoint = indexWaypoint;
        targetWaypoint = waypoint;
        needNewWaypoint = false;
    }
}
