using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuyPage
{
    FoodPage, TablePage
}

public class Buy : MonoBehaviour
{
    public GameObject foodPage;
    public GameObject tablePage;
    public Image foodBtn;
    public Image tableBtn;

    // Start is called before the first frame update
    void Start()
    {
        PageMove((int)BuyPage.FoodPage);
    }

    public void PageMove(int movePage)
    {
        Color c;
        switch ((BuyPage)movePage)
        {
            case BuyPage.FoodPage:
                foodPage.SetActive(true);
                tablePage.SetActive(false);
                c = foodBtn.color;
                c.a = 1;
                foodBtn.color = c;
                c = tableBtn.color;
                c.a = 2 / 3.0f;
                tableBtn.color = c;
                break;
            case BuyPage.TablePage:
                tablePage.SetActive(true);
                foodPage.SetActive(false);
                c = foodBtn.color;
                c.a = 2 / 3.0f;
                foodBtn.color = c;
                c = tableBtn.color;
                c.a = 1;
                tableBtn.color = c;
                break;
        }
    }
}
