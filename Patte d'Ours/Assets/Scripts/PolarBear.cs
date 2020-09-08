using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarBear : MonoBehaviour
{
    public List<Transform> Waypoints = new List<Transform>();

    Transform TargetWaypoint = null;
    int indexTargetWaypoint = 0;

    public float minDistance = 0.1f;
    int lastWaypointIndex;

    public float movementSpeed = 2.0f;

    public float rotationSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        lastWaypointIndex = Waypoints.Count;
        TargetWaypoint = Waypoints[indexTargetWaypoint];
    }

    // Update is called once per frame
    void Update()
    {
        float movementStep = movementSpeed * Time.deltaTime;

        float rotationStep = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = TargetWaypoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);

        float distance = Vector3.Distance(transform.position, TargetWaypoint.position);
        CheckDistanceToWaypoint(distance);

        transform.position = Vector3.MoveTowards(transform.position, TargetWaypoint.position, movementStep);

    }

    void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance  <= minDistance)
        {
            indexTargetWaypoint++;
            if (indexTargetWaypoint >= lastWaypointIndex)
                indexTargetWaypoint = 0;
            UpdateTargetWaypoint();
        }
    }

    void UpdateTargetWaypoint()
    {
        TargetWaypoint = Waypoints[indexTargetWaypoint];
    }
}
