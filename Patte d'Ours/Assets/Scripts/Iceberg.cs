using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iceberg : MonoBehaviour
{
    public List<Transform> WaypointList = new List<Transform>();

    public List<Transform> buildLocation = new List<Transform>();
    public List<ShopBatiments> buildType = new List<ShopBatiments>();
    public List<bool> isAlreadyBuild = new List<bool>();

    public int NumberOfBuildingPossible = 0;
}
