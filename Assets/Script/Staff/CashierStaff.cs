using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CashierStaff : Staff
{
    public Transform sellPointPos;
    private Coroutine stuffChk = null;
    public Cashier cashier;
    private bool allSellChk = false;
    private bool sellChk = false;

    protected override void WorkStateProcess()
    {
        if (myWorkState.Equals(StaffWorkState.Work))
        {
            if (stuffChk == null)
            {
                stuffChk = StartCoroutine(ChkSellPoint());
            }
        }
        else
        {
            base.WorkStateProcess();
        }
    }

    IEnumerator ChkSellPoint()
    {
        Stuff stuff = null;
        
        while (!allSellChk)
        {
            foreach (Transform t in sellPointPos)
            {
                if (t.childCount > 0)
                {
                    sellChk = true;
                    stuff = t.GetChild(0).GetComponent<Stuff>();
                    break;
                }
                yield return null;
            }
            if (stuff != null) break;
            else if (sellChk) allSellChk = true;
        }

        if (stuff != null)
        {
            yield return new WaitForSeconds(2.0f);
            cashier.AddStuff(stuff);
            StopCoroutine(stuffChk);
            stuffChk = null;
        }
        else if (allSellChk)
        {
            yield return new WaitForSeconds(2.0f);
            Debug.Log("ÆÇ´Ù");
            cashier.SellStuffs();
            StopCoroutine(stuffChk);
            stuffChk = null;
            allSellChk = false;
            sellChk = false;
        }
    }

    protected override void ResetStaff()
    {
        base.ResetStaff();
        allSellChk = false;
        sellChk = false;
        if (stuffChk != null)
        {
            StopCoroutine(ChkSellPoint());
            stuffChk = null;
        }
}
}
