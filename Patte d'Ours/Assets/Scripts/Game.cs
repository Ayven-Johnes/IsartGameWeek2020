﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Start is called before the first frame update
    void Start()
    {
        // Find the previous player's score inside Unity's player pref.
        Hearts = PlayerPrefs.GetInt("Hearts");
    }

    // Update is called once per frame
    void Update()
    {
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

    public void Buy(Batiments batiments)
    {
        int cost = 0;

        switch(batiments)
        {
            case Batiments.HOUSE:
                cost = 10;
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
}
