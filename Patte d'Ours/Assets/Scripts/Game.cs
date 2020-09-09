using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Batiments
{
    CHEST,
    BRIDGE,
    FISHDRYER,
    IGLOO1,
    IGLOO2,
    IGLOO3,
    ICEBERG,
    NONE
};

public class Game : MonoBehaviour
{ 
    [SerializeField]
    private int     HeartsPerSecond = 0;
    private float   Hearts = 0;

    [SerializeField]
    private Text HeartsDisplay = null;
    [SerializeField]
    private Text HeartsPerSecDisplay = null;

    public List<Transform> waypoints = new List<Transform>();

    public List<Bear> polarBears = new List<Bear>();

    public float distanceBetweenEachWaypoint = 50f;

    public float GetHearts { get => Hearts; set => Hearts = value; }
    public int GetHPS { get => HeartsPerSecond; set => HeartsPerSecond = value; }


    // Start is called before the first frame update
    void Start()
    {
        // Find the previous player's score inside Unity's player pref.
        Hearts = PlayerPrefs.GetInt("Hearts");
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePolarBears();
        UpdateHearts();


        // Debugging Logs
        //Debug.Log((int)Hearts);
        //Debug.Log(HeartsPerSecond);
    }

    private void UpdateHearts()
    {
        HeartsDisplay.text = "Hearts : " + ((int)Hearts).ToString();
        HeartsPerSecDisplay.text = "Hearts per second : " + HeartsPerSecond.ToString();

        Hearts += HeartsPerSecond * Time.deltaTime;

        // Save the player's "score" inside Unity's player pref.
        PlayerPrefs.SetInt("Hearts", (int)Hearts);
    }

    void UpdatePolarBears()
    {
        for (int i = 0; i < polarBears.Count; i++)
        {
            if (polarBears[i].needNewWaypoint)
            {
                ChangeWayPoint(polarBears[i]);
            }
        }
    }

    void ChangeWayPoint(Bear bear)
    {
        while (bear.needNewWaypoint)
        {
            int index = UnityEngine.Random.Range(0, waypoints.Count);
            float distance = Vector3.Distance(waypoints[index].position, bear.transform.position);
            if (index != bear.currentIndexWaypoint && distance <= distanceBetweenEachWaypoint)
                bear.UpdateWaypoint(waypoints[index], index);
        }
    }

    public void Buy(Batiments batiments, int level)
    {

    }
}
