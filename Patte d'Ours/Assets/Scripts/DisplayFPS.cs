using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    public Text fpsText = null;

    // Update is called once per frame
    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsText.text = fps.ToString();
    }
}
