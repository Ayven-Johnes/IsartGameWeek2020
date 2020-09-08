using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public Transform targetWaypoint = null;
    public int currentIndexWaypoint = 0;

    public float minDistance = 0.2f;

    public float movementSpeed = 2.0f;

    public float rotationSpeed = 2.0f;

    public int chanceToDoAction = 80;
    public bool actionLauch = false;

    public bool needNewWaypoint = true;

    NavMeshAgent agent = null;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (actionLauch)
        {
            CheckActionFinish();
        }
        else if (!needNewWaypoint)
        {
            float distance = Vector3.Distance(transform.position, agent.destination);
            CheckDistanceToWaypoint(distance);
        }
    }

    void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance <= minDistance)
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

    public void UpdateWaypoint(Vector3 waypoint, int indexWaypoint)
    {
        currentIndexWaypoint = indexWaypoint;
        needNewWaypoint = false;
        if (agent)
            agent.destination = waypoint;
    }
}
