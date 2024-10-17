using System.Collections.Generic;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    public GameObject creditCard;
    public Transform menuObj;
    public TMPro.TMP_Text count;
    public TMPro.TMP_Text price;
    public Transform sellBtn;
    public CashierGuest cashierGuest;
    public List<StuffObject> menus = new List<StuffObject>();
    private int profitGold = 0;
    int num = 1;

    public void AddStuff(Stuff stuff)
    {
        int i = 0;
        if (menus.Contains(stuff.stuff))
        {
            i = menus.IndexOf(stuff.stuff);
        }
        else
        {
            menus.Add(stuff.stuff);
            i = menus.Count - 1;
        }

        menuObj.GetChild(0).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text = stuff.myStuffDesc.menuName;
        menuObj.GetChild(1).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text =
            (int.Parse(menuObj.GetChild(1).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text) + 1).ToString();
        menuObj.GetChild(2).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text = Global.Comma(stuff.myStuffDesc.price);
        menuObj.GetChild(3).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text =
            Global.Comma(Global.UnComma(menuObj.GetChild(3).GetChild(i).GetComponentInChildren<TMPro.TMP_Text>(true).text) + stuff.myStuffDesc.price);

        menuObj.GetChild(0).GetChild(i).gameObject.SetActive(true);
        menuObj.GetChild(1).GetChild(i).gameObject.SetActive(true);
        menuObj.GetChild(2).GetChild(i).gameObject.SetActive(true);
        menuObj.GetChild(3).GetChild(i).gameObject.SetActive(true);

        count.text = (int.Parse(count.text[..^1]/*count.text.Substring(0, count.text.Length - 1)*/) + 1) + "개";

        price.text = Global.Comma(Global.UnComma(price.text[..^1]) + stuff.myStuffDesc.price) + "원";

        if (--cashierGuest.takeStuffNum == 0)
        {
            creditCard.SetActive(true);
            sellBtn.GetChild(0).gameObject.SetActive(false);
            sellBtn.GetChild(1).gameObject.SetActive(true);
        }

        profitGold += stuff.myStuffDesc.price - stuff.myStuffDesc.cost;

        Destroy(stuff.gameObject);
    }

    public void SellStuffs()
    {
        int priceGold = Global.UnComma(price.text[..^1]);
        Global.Gold += priceGold;
        Global.statistics.saleGold += priceGold;
        Global.statistics.profitGold += profitGold;
        profitGold = 0;
        Gold.Instance.OnChangeGold();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                menuObj.GetChild(i).GetChild(j).GetComponentInChildren<TMPro.TMP_Text>(true).text = "0";
                menuObj.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }
        count.text = "0개";
        price.text = "0원";

        creditCard.SetActive(false);
        sellBtn.GetChild(0).gameObject.SetActive(true);
        sellBtn.GetChild(1).gameObject.SetActive(false);
        menus.Clear();

        cashierGuest.guests.Peek().ChangeMyCurrentState(GuestCurrentState.Complete);
    }
}
