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
        public List<Sprite> LevelSprite = new List<Sprite>();
        public Text         CostText = null;
    }

    public Category currentCategory = Category.NONE;

    [SerializeField]
    private Game game = null;

    [SerializeField]
    private List<ShopItems> iglooItems = new List<ShopItems>(); // Igloo1/Igloo2/Igloo3
    [SerializeField]
    private List<ShopItems> decorationItems = new List<ShopItems>(); //Chest/Bridge/FishDryer
    [SerializeField]
    private List<int> icebergCost = new List<int>();
    [SerializeField]
    private Text icebergCostText;

    [SerializeField]
    private List<Button>    iglooButtons = new List<Button>();
    [SerializeField]
    private List<Button>    decorationButtons = new List<Button>();
    [SerializeField]
    private List<Button>    skinButtons = new List<Button>();
    [SerializeField]
    private Button          icebergButton = null;

    [SerializeField]
    private AudioClip BuySound = null;
    [SerializeField]
    private AudioClip CantBuySound = null;

    private AudioSource Source { get { return GetComponent<AudioSource>(); } }
    private int icebergCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChangeShoppingCategory(Category.IGLOO);
        foreach (ShopItems item in iglooItems)
        {
            item.CostText.text = item.Cost.ToString();
        }

        foreach (ShopItems item in decorationItems)
        {
            item.CostText.text = item.Cost.ToString();
        }

        icebergCostText.text = icebergCost[icebergCounter].ToString();
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
        ShopBatiments bat = ShopBatiments.NONE;
        switch(i)
        {
            case 0:
                bat = ShopBatiments.IGLOO1;
                break;
            case 1:
                bat = ShopBatiments.IGLOO2;
                break;
            case 2:
                bat = ShopBatiments.IGLOO3;
                break;
            default:
                break;
        }

        if(BuyItems(iglooItems[i], bat))
        {
            UpdateIglooButtonImage();
        }
    }

    public void BuyDecoration(int i)
    {
        ShopBatiments bat = ShopBatiments.NONE;
        switch (i)
        {
            case 0:
                bat = ShopBatiments.CHEST;
                break;
            case 1:
                bat = ShopBatiments.BRIDGE;
                break;
            case 2:
                bat = ShopBatiments.FISHDRYER;
                break;
            default:
                break;
        }

        if(BuyItems(decorationItems[i], bat))
        {
            UpdateDecorationImage(bat);
        }
    }

    public void BuyIceberg()
    {
        if (icebergCounter > 2)
        {
            Source.PlayOneShot(CantBuySound);
            return;
        }
        if (game.GetHearts < icebergCost[icebergCounter])
        {
            Source.PlayOneShot(CantBuySound);
            return;
        }

        Source.PlayOneShot(BuySound);
        game.GetHearts -= icebergCost[icebergCounter];

        icebergCounter++;
        if (icebergCounter <= 2)
            icebergCostText.text = icebergCost[icebergCounter].ToString();
        //Faire pop iceberg
    }

    private bool BuyItems(ShopItems item, ShopBatiments bat)
    {
        if (game.GetHearts < item.Cost || bat == ShopBatiments.NONE)
        {
            Source.PlayOneShot(CantBuySound);
            return false;
        }

        Source.PlayOneShot(BuySound);
        game.GetHearts -= item.Cost;

        switch (item.Level)
        {
            case 0:
                // game.Generate(ShopBatiments, )
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

        item.CostText.text = item.Cost.ToString();
        return true;
    }

    void UpdateIglooButtonImage()
    {
        int it = 0;
        foreach (Button bt in iglooButtons)
        {
            int level = iglooItems[it].Level;
            if (level < 10)
            {
                bt.image.sprite = iglooItems[it].LevelSprite[0];
            }
            else if (level >= 10 && level < 20)
            {
                bt.image.sprite = iglooItems[it].LevelSprite[1];
            }
            else if (level >= 20)
            {
                bt.image.sprite = iglooItems[it].LevelSprite[2];
            }
            it++;
        }
    }

    void UpdateDecorationImage(ShopBatiments type)
    {
        int it = 0;
        switch (type)
        {
            case ShopBatiments.CHEST:
                it = 0;
                break;
            case ShopBatiments.FISHDRYER:
                it = 2;
                break;
            case ShopBatiments.BRIDGE:
                it = 1;
                break;
            default:
                break;
        }

        int level = decorationItems[it].Level;
        Button bt = decorationButtons[it];
        if (level < 10)
        {
            bt.image.sprite = decorationItems[it].LevelSprite[0];
        }
        if (level >= 10 && level < 20)
        {
            bt.image.sprite = decorationItems[it].LevelSprite[1];
        }
        if (level >= 20)
        {
            bt.image.sprite = decorationItems[it].LevelSprite[2];
        }
    }
}
