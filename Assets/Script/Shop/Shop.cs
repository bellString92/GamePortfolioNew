using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Page
{
    HomePage, BuyPage, BankPage
}

public class Shop : MonoBehaviour
{
    public GameObject homePage;
    public GameObject buyPage;
    public GameObject bankPage;

    // Start is called before the first frame update
    void Start()
    {
        PageMove((int)Page.HomePage);
    }

    public void PageMove(int movePage)
    {
        switch ((Page)movePage)
        {
            case Page.HomePage:
                homePage.SetActive(true);
                buyPage.SetActive(false);
                bankPage.SetActive(false);
                break;
            case Page.BuyPage:
                buyPage.SetActive(true);
                homePage.SetActive(false);
                break;
            case Page.BankPage:
                bankPage.SetActive(true);
                homePage.SetActive(false);
                break;
        }
    }
}
