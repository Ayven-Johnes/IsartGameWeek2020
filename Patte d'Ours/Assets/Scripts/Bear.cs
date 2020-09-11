using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bear : MonoBehaviour
{
    public Transform targetWaypoint = null;

    public Animator animator = null;
    public int currentIndexWaypoint = 0;

    public float minDistance = 0.2f;

    public float movementSpeed = 2.0f;

    public float rotationSpeed = 2.0f;

    public int chanceToDoAction = 80;
    public bool isIdle = false;

    public bool needNewWaypoint = true;

    public bool isWalking = false;

    private AudioSource Source { get { return GetComponent<AudioSource>(); } }

    public List<AudioClip> FootStepClip = new List<AudioClip>();
    public List<AudioClip> BearSoundClip = new List<AudioClip>();

    NavMeshAgent agent = null;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        InvokeRepeating("RandomSound", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!needNewWaypoint && !isIdle)
        {
            float distance = Vector3.Distance(transform.position, agent.destination);
            CheckDistanceToWaypoint(distance);
        }
    }

    void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance <= minDistance)
        {
            animator.SetBool("IsWalking", false);
            
            if (Random.Range(1, 100) <= 80)
                isIdle = true;
            else 
                needNewWaypoint = true;
        }
    }

    public void UpdateWaypoint(Vector3 waypoint, int indexWaypoint)
    {
        currentIndexWaypoint = indexWaypoint;
        needNewWaypoint = false;
        if (agent)
        {
            agent.destination = waypoint;
            animator.SetBool("IsWalking", true);
        }
    }

    void RandomSound()
    {
        int rng = Random.Range(0, 20);
        if (rng == 0)
        {
            if (isWalking)
            {
                int rng2 = Random.Range(0, 6);
                Source.PlayOneShot(FootStepClip[rng2], 0.399f);
            }
            if (isIdle)
            {
                int rng2 = Random.Range(0, 9);
                Source.PlayOneShot(BearSoundClip[rng2]);
            }
        }
    }
}
