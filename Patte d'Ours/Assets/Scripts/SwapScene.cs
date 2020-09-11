using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwapScene : MonoBehaviour
{
    public float timeBeforeSwap = 5;
    float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= timeBeforeSwap)
            SceneManager.LoadScene("Leo's Scene2");
    }
}
