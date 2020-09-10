using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public enum Batiments
{
    CHEST,
    BRIDGE1,
    BRIDGE2,
    BRIDGE3,
    FISHDRYER,
    IGLOO1,
    IGLOO2,
    IGLOO3,
    ICEBERG,
    SECHOIR1,
    SECHOIR2,
    SECHOIR3,
    CHEST1,
    CHEST2,
    CHEST3,
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
    public GameObject bearPrefab = null;
    public GameObject babyBearPrefab = null;
    public float rangeSpawnBear = 30f;

    public float distanceBetweenEachWaypoint = 50f;

    public float GetHearts { get => Hearts; set => Hearts = value; }
    public int GetHPS { get => HeartsPerSecond; set => HeartsPerSecond = value; }
    #region BuildingVars

    public List<Transform> buildingLocationPossible = new List<Transform>();
    public List<Batiments> buildingType = new List<Batiments>();
    public List<bool> AlreadyBuild = new List<bool>();
    public List<GameObject> buildingPrefab = new List<GameObject>();

    public List<GameObject> buildingList = new List<GameObject>();
    public List<Batiments> buildingTypesInList = new List<Batiments>();

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

        for (int i = 0; i < buildingType.Count; i++)
        {
            if (buildingType[i] == Batiments.BRIDGE1 && !AlreadyBuild[i])
            {

                AlreadyBuild[i] = true;
                GameObject newHouse = Instantiate(buildingPrefab[9]);
                newHouse.transform.position = buildingLocationPossible[i].position;
                newHouse.transform.rotation = buildingLocationPossible[i].rotation;

                buildingList.Add(newHouse);
                buildingTypesInList.Add(Batiments.BRIDGE1);
            }
            else if (buildingType[i] != Batiments.BRIDGE1 && !AlreadyBuild[i] && i == buildingType.Count)
                return;
        }
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
                bear.UpdateWaypoint(waypoints[index].position, index);
        }
    }

    public void Buy(Batiments batiments, int level)
    {

    }

    public void GenerateOneIceberg()
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

        if (ice.WaypointList.Count != 0)
        {
            foreach (Transform point in ice.WaypointList)
            {
                waypoints.Add(point);
            }
        }

        for (int i = 0; i < ice.NumberOfBuildingPossible; i++)
        {
            buildingLocationPossible.Add(ice.buildLocation[i]);
            buildingType.Add(ice.buildType[i]);
            AlreadyBuild.Add(ice.isAlreadyBuild[i]);

            if (ice.buildType[i] == Batiments.BRIDGE1)
            {
                AlreadyBuild[i] = true;

                GameObject newBridge = Instantiate(buildingPrefab[9]);
                newBridge.transform.position = ice.buildLocation[i].position;
                newBridge.transform.rotation = ice.buildLocation[i].rotation;

                buildingList.Add(newBridge);
                buildingTypesInList.Add(Batiments.BRIDGE1);
            }
        }
    }

    public void GenerateItemUpgrade1(Batiments bat)
    {
        switch (bat)
        {
            case Batiments.CHEST1:
                GenerateChest1();
                break;
            case Batiments.SECHOIR1:
                GenerateSechoir1();
                break;
            case Batiments.IGLOO1:
                GenerateIgloo1();
                break;
            default:
                break;
        }
    }

    public void GenerateItemUpgrade2(Batiments bat)
    {
        switch (bat)
        {
            case Batiments.CHEST2:
                GenerateChest2();
                break;
            case Batiments.SECHOIR2:
                GenerateSechoir2();
                break;
            case Batiments.IGLOO2:
                GenerateIgloo2();
                break;
            case Batiments.BRIDGE2:
                GenerateBridge2();
                break;
            default:
                break;
        }
    }

    public void GenerateItemUpgrade3(Batiments bat)
    {
        switch (bat)
        {
            case Batiments.CHEST3:
                GenerateChest3();
                break;
            case Batiments.SECHOIR3:
                GenerateSechoir3();
                break;
            case Batiments.IGLOO3:
                GenerateIgloo3();
                break;
            case Batiments.BRIDGE3:
                GenerateBridge3();
                break;
            default:
                break;
        }
    }

    public void GenerateIgloo1()
    {
        int index = -1;
        for (int i = 0; i < buildingType.Count; i++)
        {
            if (buildingType[i] == Batiments.IGLOO1 && !AlreadyBuild[i])
            {
                index = i;
                AlreadyBuild[index] = true;
                GameObject newHouse = Instantiate(buildingPrefab[0]);
                newHouse.transform.position = buildingLocationPossible[index].position;
                newHouse.transform.rotation = buildingLocationPossible[index].rotation;

                buildingList.Add(newHouse);
                buildingTypesInList.Add(Batiments.IGLOO1);

                GameObject newBear = null;
                if (UnityEngine.Random.Range(1, 100) < 25)
                    newBear = Instantiate(babyBearPrefab);
                else
                    newBear = Instantiate(bearPrefab);

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
            else if (buildingType[i] != Batiments.IGLOO1 && !AlreadyBuild[i] && i == buildingType.Count)
                return;
        }
    }

    public void GenerateIgloo2()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.IGLOO1)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[1]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.IGLOO2;

                Destroy(oldBuilding);

                GameObject newBear = Instantiate(bearPrefab);

                bool indexIsOk = false; int j = -1;
                while (!indexIsOk)
                {
                    j = UnityEngine.Random.Range(0, waypoints.Count);
                    float distance = Vector3.Distance(waypoints[j].position, newbuilding.transform.position);
                    if (distance < rangeSpawnBear)
                    {
                        indexIsOk = true;
                    }
                }

                newBear.transform.position = waypoints[j].position;
                polarBears.Add(newBear.GetComponent<Bear>());
            }
        }
    }

    public void GenerateIgloo3()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.IGLOO2)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[2]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.IGLOO3;

                Destroy(oldBuilding);

                GameObject newBear = Instantiate(bearPrefab);

                bool indexIsOk = false; int j = -1;
                while (!indexIsOk)
                {
                    j = UnityEngine.Random.Range(0, waypoints.Count);
                    float distance = Vector3.Distance(waypoints[j].position, newbuilding.transform.position);
                    if (distance < rangeSpawnBear)
                    {
                        indexIsOk = true;
                    }
                }

                newBear.transform.position = waypoints[j].position;
                polarBears.Add(newBear.GetComponent<Bear>());

            }
        }
    }

    public void GenerateSechoir1()
    {
        int index = -1;
        for (int i = 0; i < buildingType.Count; i++)
        {
            if (buildingType[i] == Batiments.SECHOIR1 && !AlreadyBuild[i])
            {
                index = i;
                AlreadyBuild[index] = true;
                GameObject newSechoir = Instantiate(buildingPrefab[3]);
                newSechoir.transform.position = buildingLocationPossible[index].position;
                newSechoir.transform.rotation = buildingLocationPossible[index].rotation;

                buildingList.Add(newSechoir);
                buildingTypesInList.Add(Batiments.SECHOIR1);

                return;
            }
            else if (buildingType[i] != Batiments.SECHOIR1 && !AlreadyBuild[i] && i == buildingType.Count)
                return;
        }
    }

    public void GenerateSechoir2()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.SECHOIR1)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[4]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.SECHOIR2;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateSechoir3()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.SECHOIR2)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[5]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.SECHOIR3;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateChest1()
    {
        int index = -1;
        for (int i = 0; i < buildingType.Count; i++)
        {
            if (buildingType[i] == Batiments.CHEST1 && !AlreadyBuild[i])
            {
                index = i;
                AlreadyBuild[index] = true;
                GameObject newSechoir = Instantiate(buildingPrefab[6]);
                newSechoir.transform.position = buildingLocationPossible[index].position;
                newSechoir.transform.rotation = buildingLocationPossible[index].rotation;

                buildingList.Add(newSechoir);
                buildingTypesInList.Add(Batiments.CHEST1);

                return;
            }
            else if (buildingType[i] != Batiments.CHEST1 && !AlreadyBuild[i] && i == buildingType.Count)
                return;
        }
    }

    public void GenerateChest2()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.CHEST1)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[7]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.CHEST2;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateChest3()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.CHEST2)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[8]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.CHEST3;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateBridge2()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.BRIDGE1)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[10]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.BRIDGE2;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateBridge3()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.BRIDGE2)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[11]);

                newbuilding.transform.position = oldBuilding.transform.position;
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.BRIDGE3;

                Destroy(oldBuilding);
            }
        }
    }
}
