using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public enum Category
    {
        IGLOO,
        ICEBERG,
        DECORATION,
        SKINS,
        NONE
    }

    [Serializable]
    public class ShopItems
    {
        public  string      Name = "";
        public  int         Cost = 0;
        public  float       CostMultiplier = 0f;
        public  int         Gain = 0;
        public  float       GainMultiplier = 0f;
        public  int         Level = 0;
    }

    public Category currentCategory = Category.NONE;

    [SerializeField]
    private Game game = null;

    [SerializeField]
    private List<ShopItems> iglooItems = new List<ShopItems>();
    [SerializeField]
    private List<ShopItems> decorationItems = new List<ShopItems>();
    [SerializeField]
    private List<int> icebergCost = new List<int>();

    [SerializeField]
    private List<Button>    iglooButtons = new List<Button>();
    [SerializeField]
    private List<Button>    decorationButtons = new List<Button>();
    [SerializeField]
    private List<Button>    skinButtons = new List<Button>();
    [SerializeField]
    private Button          icebergButton = null;

    private int icebergCounter = 0;

    int tmp = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChangeShoppingCategory(Category.IGLOO);
    }

    private void ChangeShoppingCategory(Category cat)
    {
        if (cat != currentCategory)
        {
            Debug.Log("Not same category");
            foreach (Button bt in iglooButtons)
            {
                bt.enabled = false;
                bt.gameObject.SetActive(false);
            }
            foreach (Button bt in decorationButtons)
            {
                bt.enabled = false;
                bt.gameObject.SetActive(false);
            }
            foreach (Button bt in skinButtons)
            {
                bt.enabled = false;
                bt.gameObject.SetActive(false);
            }
            icebergButton.enabled = false;
            icebergButton.gameObject.SetActive(false);


            switch (cat)
            {
                case Category.IGLOO:
                {
                    foreach(Button bt in iglooButtons)
                    {
                        bt.enabled = true;
                        bt.gameObject.SetActive(true);
                        }
                    }
                    break;
                case Category.DECORATION:
                {
                    foreach (Button bt in decorationButtons)
                    {
                        bt.enabled = true;
                        bt.gameObject.SetActive(true);
                        }
                    }
                    break;
                case Category.SKINS:
                {
                    foreach (Button bt in skinButtons)
                    {
                        bt.enabled = true;
                        bt.gameObject.SetActive(true);
                        }
                    }
                    break;
                case Category.ICEBERG:
                    icebergButton.enabled = true;
                    icebergButton.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            currentCategory = cat;
        }
    }

    public void ChangeToIgloo()
    {
        ChangeShoppingCategory(Category.IGLOO);
    }

    public void ChangeToDecoration()
    {
        ChangeShoppingCategory(Category.DECORATION);
    }

    public void ChangeToSkin()
    {
        ChangeShoppingCategory(Category.SKINS);
    }

    public void ChangeToIceberg()
    {
        ChangeShoppingCategory(Category.ICEBERG);
    }

    public void BuyIgloo(int i)
    {
        BuyItems(iglooItems[i]);
        Debug.Log("buy igloo 1");
    }

    public void BuyDecoration(int i)
    {
        //BuyItems(decorationItems[i]);

        if ( tmp == 0)
        {
            game.GenerateSechoir();
            game.GenerateOneIceberg();
            game.GenerateOneIceberg();
            game.GenerateOneIceberg();
        }
        if (tmp == 1)
        {
            game.GenerateSechoir();
        }
        if (tmp == 2)
        {
            game.sechoirLevel = 2;
            game.GenerateSechoir();
        }
        if (tmp == 3)
        {
            game.sechoirLevel = 3;
            game.GenerateSechoir();
        }
        tmp++;
    }

    public void BuyIceberg()
    {
        if (game.GetHearts < icebergCost[icebergCounter] || icebergCounter > 2)
            return;

        game.GetHearts -= icebergCost[icebergCounter];
        icebergCounter++;

        game.GenerateOneIceberg();
    }

    private void BuyItems(ShopItems item)
    {
        if (game.GetHearts < item.Cost)
            return;

 
        game.GetHearts -= item.Cost;

        switch (item.Level)
        {
            case 0:
                // Generate 
                break;
            case 9:
                //Upgrade 1
                break;
            case 19:
                //Upgrade 2
                break;
        }

        if(item.Level == 0)
        {
            game.GetHPS += item.Gain;
        }
        else
        {
            game.GetHPS -= item.Gain;
            game.GetHPS += (int)(item.Gain * item.GainMultiplier);
        }

        item.Level++;
        item.Gain = (int)(item.Gain * item.GainMultiplier);
        item.Cost = (int)(item.Cost * item.CostMultiplier);
    }
}
