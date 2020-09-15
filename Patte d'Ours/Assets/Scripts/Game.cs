using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public enum ShopBatiments
{
    CHEST,
    FISHDRYER,
    IGLOO1,
    IGLOO2,
    IGLOO3,
    BRIDGE,
    ICEBERG,
    NONE
}

public enum Batiments
{
    BRIDGE1,
    BRIDGE2,
    BRIDGE3,
    IGLOO1,
    IGLOO2,
    IGLOO3,
    SECHOIR1,
    SECHOIR2,
    SECHOIR3,
    CHEST1,
    CHEST2,
    CHEST3,
    OPENCHEST1,
    OPENCHEST2,
    OPENCHEST3,
    NONE
};

public class Game : MonoBehaviour
{
    [SerializeField]
    private int HeartsPerSecond = 5;
    private float Hearts = 0;

    [SerializeField]
    private int[] ClickGain = new int[4];

    [SerializeField]
    private Text HeartsDisplay = null;
    [SerializeField]
    private Text HeartsPerSecDisplay = null;

    public List<Transform> waypoints = new List<Transform>();
    public List<Bear> polarBears = new List<Bear>();
    public GameObject bearPrefab = null;
    public GameObject babyBearPrefab = null;
    public float rangeSpawnBear = 30f;

    public List<Transform> beginSpawnBear = new List<Transform>();

    public float distanceBetweenEachWaypoint = 50f;

    public float GetHearts { get => Hearts; set => Hearts = value; }
    public int GetHPS { get => HeartsPerSecond; set => HeartsPerSecond = value; }

    #region BuildingVars

    public List<Transform> buildingLocationPossible = new List<Transform>();

    public List<Transform> IglooLocations = new List<Transform>();

    public List<Batiments> buildingType = new List<Batiments>();
    public List<bool> AlreadyBuild = new List<bool>();
    public List<GameObject> buildingPrefab = new List<GameObject>();

    public List<GameObject> buildingList = new List<GameObject>();
    public List<Batiments> buildingTypesInList = new List<Batiments>();

    public int bridgeLevel = 1;

    public int openChestOnMap = 0;
    public int openChestMaxOnMap = 1;
    public int openChestLevel = 0;

    public int sechoirLevel = 0;
    public int sechoirOnMap = 0;
    public int sechoirMaxOnMap = 2;

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

        SpawnBearPos(beginSpawnBear[0].position);
        SpawnBearPos(beginSpawnBear[1].position);
        SpawnBearPos(beginSpawnBear[2].position);
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
        int i = 0;
        while (bear.needNewWaypoint)
        {
            int index = UnityEngine.Random.Range(0, waypoints.Count);
            float distance = Vector3.Distance(waypoints[index].position, bear.transform.position);

            if (index != bear.currentIndexWaypoint && distance <= distanceBetweenEachWaypoint)
                bear.UpdateWaypoint(waypoints[index].position, index);
            if (i > 100)
            {
                bear.UpdateWaypoint(waypoints[0].position, index);
            }
        }
    }

    public void GenerateOneIceberg()
    {
        if (numberIcebergGenerate >= icebergLocationPossible.Count)
            return;

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

        for (int i = 0; i < 3; i++)
            IglooLocations.Add(ice.IglooLocations[i]);

        for (int i = 0; i < ice.NumberOfBuildingPossible; i++)
        {
            buildingLocationPossible.Add(ice.buildLocation[i]);
            buildingType.Add(ice.buildType[i]);
            AlreadyBuild.Add(ice.isAlreadyBuild[i]);

            if (ice.buildType[i] == Batiments.BRIDGE1)
            {
                if (bridgeLevel == 1)
                {
                    AlreadyBuild[i] = true;

                    GameObject newBridge = Instantiate(buildingPrefab[9]);
                    newBridge.transform.position = ice.buildLocation[i].position;
                    newBridge.transform.rotation = ice.buildLocation[i].rotation;

                    buildingList.Add(newBridge);
                    buildingTypesInList.Add(Batiments.BRIDGE1);
                }
                else if (bridgeLevel == 2)
                {
                    AlreadyBuild[i] = true;

                    GameObject newBridge = Instantiate(buildingPrefab[10]);
                    newBridge.transform.position = ice.buildLocation[i].position + new Vector3(0, 31.33f, 0);
                    newBridge.transform.rotation = ice.buildLocation[i].rotation;

                    buildingList.Add(newBridge);
                    buildingTypesInList.Add(Batiments.BRIDGE2);
                }
                else if (bridgeLevel == 3)
                {
                    AlreadyBuild[i] = true;

                    GameObject newBridge = Instantiate(buildingPrefab[11]);
                    newBridge.transform.position = ice.buildLocation[i].position;
                    newBridge.transform.rotation = ice.buildLocation[i].rotation;

                    buildingList.Add(newBridge);
                    buildingTypesInList.Add(Batiments.BRIDGE3);
                }
            }

            if (ice.buildType[i] == Batiments.CHEST1)
            {
                if (openChestOnMap < openChestMaxOnMap)
                {
                    Debug.Log("Generate One Chest at the spawn iceberg");
                    if (openChestLevel == 1)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[6]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;
                        openChestOnMap++;
                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.CHEST1);
                    }
                    else if (openChestLevel == 2)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[7]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;
                        openChestOnMap++;

                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.CHEST2);
                    }
                    else if (openChestLevel == 3)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[8]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;
                        openChestOnMap++;

                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.CHEST3);
                    }
                }
            }

            if (ice.buildType[i] == Batiments.SECHOIR1)
            {
                if (sechoirOnMap < sechoirMaxOnMap)
                {
                    if (sechoirLevel == 1)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[3]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;

                        sechoirOnMap++;

                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.SECHOIR1);
                    }
                    else if (sechoirLevel == 2)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[4]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;
                        sechoirOnMap++;

                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.SECHOIR2);
                    }
                    else if (sechoirLevel == 3)
                    {
                        AlreadyBuild[i] = true;

                        GameObject newBridge = Instantiate(buildingPrefab[5]);
                        newBridge.transform.position = ice.buildLocation[i].position;
                        newBridge.transform.rotation = ice.buildLocation[i].rotation;
                        sechoirOnMap++;

                        buildingList.Add(newBridge);
                        buildingTypesInList.Add(Batiments.SECHOIR3);
                    }
                }
            }
        }
    }

    public void SpawnBear(GameObject newHouse)
    {
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

        GameObject newBear = null;
        if (UnityEngine.Random.Range(1, 100) < 25)
            newBear = Instantiate(babyBearPrefab, waypoints[j].position, new Quaternion());
        else
            newBear = Instantiate(bearPrefab, waypoints[j].position, new Quaternion());
        newBear.GetComponent<Bear>().needNewWaypoint = true;
        polarBears.Add(newBear.GetComponent<Bear>());
    }

    public void SpawnBearPos(Vector3 pos)
    {
        GameObject newBear = null;
        if (UnityEngine.Random.Range(1, 100) < 25)
            newBear = Instantiate(babyBearPrefab, pos, new Quaternion());
        else
            newBear = Instantiate(bearPrefab, pos, new Quaternion());

        polarBears.Add(newBear.GetComponent<Bear>());
    }

    public void GenerateIgloo1(int numb, int level)
    {
        if (level == 0)
        {
            GameObject newHouse = Instantiate(buildingPrefab[0]);
            newHouse.transform.position = IglooLocations[0].position;
            newHouse.transform.rotation = IglooLocations[0].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 9)
        {
            GameObject newHouse = Instantiate(buildingPrefab[0]);
            newHouse.transform.position = IglooLocations[1].position;
            newHouse.transform.rotation = IglooLocations[1].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 19)
        {
            GameObject newHouse = Instantiate(buildingPrefab[0]);
            newHouse.transform.position = IglooLocations[2].position;
            newHouse.transform.rotation = IglooLocations[2].rotation;
            SpawnBear(newHouse);
        }
    }

    public void GenerateIgloo2(int numb, int level)
    {
        if (level == 0)
        {
            GameObject newHouse = Instantiate(buildingPrefab[1]);
            newHouse.transform.position = IglooLocations[0].position;
            newHouse.transform.rotation = IglooLocations[0].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 9)
        {
            GameObject newHouse = Instantiate(buildingPrefab[1]);
            newHouse.transform.position = IglooLocations[1].position;
            newHouse.transform.rotation = IglooLocations[1].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 19)
        {
            GameObject newHouse = Instantiate(buildingPrefab[1]);
            newHouse.transform.position = IglooLocations[2].position;
            newHouse.transform.rotation = IglooLocations[2].rotation;
            SpawnBear(newHouse);
        }
    }

    public void GenerateIgloo3(int numb, int level)
    {
        if (level == 0)
        {
            GameObject newHouse = Instantiate(buildingPrefab[2]);
            newHouse.transform.position = IglooLocations[0].position;
            newHouse.transform.rotation = IglooLocations[0].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 9)
        {
            GameObject newHouse = Instantiate(buildingPrefab[2]);
            newHouse.transform.position = IglooLocations[1].position;
            newHouse.transform.rotation = IglooLocations[1].rotation;
            SpawnBear(newHouse);
        }
        else if (level == 19)
        {
            GameObject newHouse = Instantiate(buildingPrefab[2]);
            newHouse.transform.position = IglooLocations[2].position;
            newHouse.transform.rotation = IglooLocations[2].rotation;
            SpawnBear(newHouse);
        }
    }

    public void GenerateSechoir()
    {
        if (sechoirLevel == 1)
            GenerateSechoir1();
        else if (sechoirLevel == 2)
        {
            GenerateSechoir1();
            GenerateSechoir2();
        }
        else if (sechoirLevel == 3)
        {
            GenerateSechoir1();
            GenerateSechoir2();
            GenerateSechoir3();
        }
    }

    public void GenerateChest()
    {
        if (openChestLevel == 1)
            GenerateChest1();
        else if (openChestLevel == 2)
        {
            GenerateChest1();
            GenerateChest2();
        }
        else if (openChestLevel == 3)
        {
            GenerateChest1();
            GenerateChest2();
            GenerateChest3();
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
                sechoirOnMap++;

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
                openChestOnMap++;
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

                newbuilding.transform.position = oldBuilding.transform.position + new Vector3(-0.13f, 0f, 1.67f);
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

                newbuilding.transform.position = oldBuilding.transform.position - new Vector3(-0.13f, 0f, 1.67f) + new Vector3(-0.26f, 1.69f, -1.68f);
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.CHEST3;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateBridge2()
    {
        bridgeLevel = 2;
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingTypesInList[i] == Batiments.BRIDGE1)
            {
                GameObject oldBuilding = buildingList[i];
                GameObject newbuilding = Instantiate(buildingPrefab[10]);

                newbuilding.transform.position = oldBuilding.transform.position + new Vector3(0, 1.41f, 0);
                newbuilding.transform.rotation = oldBuilding.transform.rotation;

                buildingList[i] = newbuilding;
                buildingTypesInList[i] = Batiments.BRIDGE2;

                Destroy(oldBuilding);
            }
        }
    }

    public void GenerateBridge3()
    {
        bridgeLevel = 3;
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

    public void Click()
    {
        Source.PlayOneShot(ClickerSound);
        Hearts += ClickGain[numberIcebergGenerate];
    }
}
