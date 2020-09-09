﻿using System;
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

    [SerializeField]
    private AudioClip ButtonBuySound = null;

    private AudioSource Source { get { return GetComponent<AudioSource>(); } }
    private int icebergCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChangeShoppingCategory(Category.IGLOO);
    }

    private void ChangeShoppingCategory(Category cat)
    {
        if (cat != currentCategory)
        {
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
        Batiments bat = Batiments.NONE;
        switch(i)
        {
            default:
                break;
        }

        BuyItems(iglooItems[i], bat);
    }

    public void BuyDecoration(int i)
    {
        Batiments bat = Batiments.NONE;
        switch (i)
        {
            default:
                break;
        }
        BuyItems(decorationItems[i], bat);
    }

    public void BuyIceberg()
    {
        if (game.GetHearts < icebergCost[icebergCounter] || icebergCounter > 2)
            return;

        Source.PlayOneShot(ButtonBuySound);
        game.GetHearts -= icebergCost[icebergCounter];
        icebergCounter++;
        //Faire pop iceberg
    }

    private void BuyItems(ShopItems item, Batiments bat)
    {
        if (game.GetHearts < item.Cost || bat == Batiments.NONE)
            return;

        Source.PlayOneShot(ButtonBuySound);
        game.GetHearts -= item.Cost;

        switch (item.Level)
        {
            case 0:
                //Faire pop
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
