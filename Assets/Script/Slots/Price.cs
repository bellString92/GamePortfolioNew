using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Price : MonoBehaviour
{
    public Image stuffImg;
    public TMPro.TMP_Text stuffDesc;
    public TMPro.TMP_InputField price;
    public TMPro.TMP_Text profit;
    private int cost = 0;
    private int oriPrice;
    public Player player;

    public void OnOpenPricePopup(int price, StuffObject stuff)
    {
        this.oriPrice = price;
        stuffImg.sprite = Resources.Load<Sprite>($"Images/Stuff/{stuff}");
        string desc = "";
        Stuff obj = Global.GetStuff(stuff);
        desc += $"이름 : {obj.myStuffDesc.menuName}\n";
        desc += $"종류 : {obj.myStuffDesc.kind}\n";
        this.cost = obj.myStuffDesc.cost;
        desc += $"단가\n{Global.Comma(cost)}원";
        stuffDesc.text = desc;
        this.price.text = price.ToString();
        profit.text = Global.Comma(price - cost) + "원";
        gameObject.SetActive(true);
    }

    public void OnChangePrice()
    {
        if (int.TryParse(this.price.text, out int price))
        {
            profit.text = Global.Comma(price - cost) + "원";
        }
    }

    public void OnExit()
    {
        player.OffPricePopup(oriPrice);
    }

    public void OnSave()
    {
        if (int.TryParse(this.price.text, out int price))
        {
            player.OffPricePopup(price);
        }
    }
}
