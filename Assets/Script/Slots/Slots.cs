using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public StuffObject myStuff;
    public Transform[] slotArr = null;
    public int count = 0;
    public bool countChk = true;

    private void Start()
    {
        myStuff = StuffObject.None;
        slotArr = new Transform[10];
        int i = 0;
        foreach (Transform t in transform.GetChild(0))
        {
            slotArr[i++] = t;
        }
    }

    private void ChangeMyStuff(StuffObject stuffObject)
    {
        myStuff = stuffObject;
    }

    private void ClearMyStuff()
    {
        myStuff = StuffObject.None;
    }

    public bool OnDisplayCheck(StuffObject stuff)
    {
        bool chk = false;
        if ((myStuff.Equals(StuffObject.None) || myStuff.Equals(stuff)) && countChk) chk = true;


        if (0 < (int)stuff && (int)stuff <= 10)
        {
            if ((LayerMask.GetMask("Shelf") & (1 << gameObject.layer)) == 0) chk = false;
        }
        else if (10 < (int)stuff && (int)stuff <= 20)
        {
            if ((LayerMask.GetMask("Refrigerator") & (1 << gameObject.layer)) == 0) chk = false;
        }

        return chk;
    }

    public void AddStuff(Transform t)
    {
        if (count == 0) ChangeMyStuff(t.GetComponent<Stuff>().stuff);
        t.SetParent(slotArr[count++]);
        t.localScale = new Vector3(2, 40, 1);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.gameObject.SetActive(true);
        if (count >= 10) countChk = false;
    }

    public void PutStuff(Transform t)
    {
        
    }
}
