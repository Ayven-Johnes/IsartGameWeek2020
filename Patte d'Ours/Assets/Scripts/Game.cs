﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public enum ShopBatiments
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
    private int     HeartsPerSecond = 5;
    private float   Hearts = 0;

    [SerializeField]
    private int[] ClickGain = new int[4];

    [SerializeField]
    private Text HeartsDisplay = null;
    [SerializeField]
    private Text HeartsPerSecDisplay = null;

    public List<Transform> waypoints = new List<Transform>();
    public List<Bear> polarBears = new List<Bear>();
    public GameObject bearPrefab = null;
    public float rangeSpawnBear = 30f;
    public float distanceBetweenEachWaypoint = 50f;

    public float GetHearts { get => Hearts; set => Hearts = value; }
    public int GetHPS { get => HeartsPerSecond; set => HeartsPerSecond = value; }

    #region BuildingVars

    public List<Transform> buildingLocationPossible = new List<Transform>();
    public List<ShopBatiments> buildingType = new List<ShopBatiments>();
    public List<bool> AlreadyBuild = new List<bool>();
    public List<GameObject> buildingPrefab = new List<GameObject>();

    [SerializeField]
    private AudioClip ClickerSound = null;
    private AudioSource Source { get { return GetComponent<AudioSource>(); } }

    #endregion

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
    }

    private void UpdateHearts()
    {
        HeartsDisplay.text = ((int)Hearts).ToString();
        HeartsPerSecDisplay.text = HeartsPerSecond.ToString();

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
                bear.UpdateWaypoint(waypoints[index].position, index);
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

        GameObject newIceBerg = Instantiate(prefabForEachIceberg[index]);
        newIceBerg.transform.position = icebergLocationPossible[index].position;
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

        for (int i = 0; i < ice.NumberOfBuildingPossible; i++)
        {
            buildingLocationPossible.Add(ice.buildLocation[i]);
            buildingType.Add(ice.buildType[i]);
            AlreadyBuild.Add(ice.isAlreadyBuild[i]);
        }
    }

    void GenerateHouse()
    {
        int index = -1;
        for (int i = 0; i < buildingType.Count; i++)
        {
            if (buildingType[i] == ShopBatiments.IGLOO1 && !AlreadyBuild[i])
            {
                index = i;
                AlreadyBuild[index] = true;
                GameObject newHouse = Instantiate(buildingPrefab[0]);
                newHouse.transform.position = buildingLocationPossible[index].position;
                newHouse.transform.rotation = buildingLocationPossible[index].rotation;

                GameObject newBear = Instantiate(bearPrefab);

                bool indexIsOk = false; int j = -1;
                while (!indexIsOk)
                {
                    j = UnityEngine.Random.Range(0, waypoints.Count);
                    float distance = Vector3.Distance(waypoints[j].position, newHouse.transform.position);
                    if (distance < rangeSpawnBear)
                    {
                        indexIsOk = true;
                    }
                }

                newBear.transform.position = waypoints[j].position;
                polarBears.Add(newBear.GetComponent<Bear>());
                return;
            }
            else if (buildingType[i] != ShopBatiments.IGLOO1 && !AlreadyBuild[i] && i == buildingType.Count)
                return;
        }

    }

    public void Click()
    {
        Source.PlayOneShot(ClickerSound);
        Hearts += ClickGain[numberIcebergGenerate];
    }
}
