using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CashierStaff : Staff
{
    public Transform sellPointPos;
    private Coroutine stuffChk = null;
    public Cashier cashier;

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
        while (true)
        {
            foreach (Transform t in sellPointPos)
            {
                if (t.childCount > 0)
                {
                    stuff = t.GetChild(0).GetComponent<Stuff>();
                    break;
                }
                yield return null;
            }
            if (stuff != null) break;
        }

        cashier.AddStuff(stuff);
        yield return new WaitForSeconds(2.0f);
        StopCoroutine(stuffChk);
        stuffChk = null;
    }
}
