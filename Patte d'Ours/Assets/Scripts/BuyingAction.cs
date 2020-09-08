using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingAction : MonoBehaviour
{
    [SerializeField]
    private Batiments BatimentToBuy = Batiments.NONE;
    [SerializeField]
    private Game Manager = null;

    public void Buy()
    {
        if(!Manager)
        {
            Debug.LogError("No Game Manager assign to" + gameObject.name);
        }

        Manager.Buy(BatimentToBuy);
    }
}
