using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public enum Batiments
{
    HOUSE = 0,
    SCHOOL = 1,
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

    #region IcebergGeneration

    public List<Transform> icebergLocationPossible = new List<Transform>();
    List<bool> icebergAlreadyGenerate = new List<bool>();

    public List<GameObject> prefabForEachIceberg = new List<GameObject>();

    int numberIcebergGenerate = 0;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Find the previous player's score inside Unity's player pref.
        Hearts = PlayerPrefs.GetInt("Hearts");

        for (int i = 0; i <= icebergLocationPossible.Count; i++)
        {
            icebergAlreadyGenerate.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePolarBears();
        UpdateHearts();


        // Debugging Logs
        Debug.Log((int)Hearts);
        Debug.Log(HeartsPerSecond);
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

    public void Buy(Batiments batiments)
    {
        int cost = 0;

        switch(batiments)
        {
            case Batiments.HOUSE:
                cost = 10;
                GenerateOneIceberg();
                break;
            case Batiments.SCHOOL:
                cost = 20;
                break;
        }

        if (cost > 0 && cost <= Hearts)
        {
            Hearts -= cost;
        }
    }

    void GenerateOneIceberg()
    {
        if (numberIcebergGenerate >= icebergLocationPossible.Count)
        {
            return;
        }

        bool indexIsOk = false; int index = -1;
        while (!indexIsOk)
        {
            index = UnityEngine.Random.Range(0, icebergLocationPossible.Count);
            if (!icebergAlreadyGenerate[index])
            {
                indexIsOk = true;
                icebergAlreadyGenerate[index] = true;
            }
        }

        GameObject newIceBerg = Instantiate(prefabForEachIceberg[index], icebergLocationPossible[index]);
        GetInformationOfIceberg(newIceBerg);

        numberIcebergGenerate++;
    }

    void GetInformationOfIceberg(GameObject newIceberg)
    {
        Iceberg ice = newIceberg.GetComponent<Iceberg>();

        foreach (Transform point in ice.WaypointList)
        {
            waypoints.Add(point);
        }
    }
}
