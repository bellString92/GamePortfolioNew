using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public StuffObject myStuff;
    public Transform[] slotArr = null;
    public int maxCount = 10;
    public int count = 0;
    public bool countChk = true;
    public GameObject priceObj;
    public int price = 0;

    private void Start()
    {
        myStuff = StuffObject.None;
        slotArr = new Transform[maxCount];
        int i = 0;
        foreach (Transform t in transform.GetChild(0))
        {
            slotArr[i++] = t;
        }
    }

    private void ChangeMyStuff(StuffObject stuffObject, int price)
    {
        myStuff = stuffObject;
        this.price = price;
        priceObj.GetComponentInChildren<TMPro.TMP_Text>().text = Global.Comma(price) + "¿ø";
        priceObj.SetActive(true);
    }

    private void ClearMyStuff()
    {
        myStuff = StuffObject.None;
        this.price = 0;
        priceObj.SetActive(false);
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

    public bool AddStuff(Transform t)
    {
        if (count == 0) ChangeMyStuff(t.GetComponent<Stuff>().stuff, Global.GetStuff(t.GetComponent<Stuff>().stuff).myStuffDesc.price);
        bool chk = false;
        for (int i = 0; i < slotArr.Length; i++)
        {
            if (slotArr[i].childCount == 0)
            {
                t.SetParent(slotArr[i]);
                t.localScale = new Vector3(2, 40, 1);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.gameObject.SetActive(true);
                if (++count >= maxCount) countChk = false;
                chk = true;
                break;
            }
        }
        return chk;
    }

    public void OnChangePrice(int price)
    {
        if (this.price != price)
        {
            Global.SetStuffPrice(myStuff, price);
            foreach (GameObject s in GameObject.FindGameObjectsWithTag("Slot"))
            {
                if (s.GetComponent<Slots>().myStuff.Equals(myStuff))
                {
                    s.GetComponent<Slots>().price = price;
                    foreach (Transform t in s.transform.GetChild(0).transform)
                    {
                        if (t.childCount > 0)
                        {
                            t.GetComponentInChildren<Stuff>().myStuffDesc.price = price;
                        }
                    }
                    s.GetComponent<Slots>().GetComponentInChildren<TMPro.TMP_Text>().text = Global.Comma(price) + "¿ø";
                }
            }
        }
    }

    public Transform PutStuff(bool take)
    {
        if (count <= 0) return null;

        for (int i = 0; i < slotArr.Length; i++)
        {
            if (slotArr[i].childCount > 0)
            {
                Transform stuff = slotArr[i];
                if (take)
                {
                    count--;
                    countChk = true;
                    if (count == 0) ClearMyStuff();
                }
                return stuff;
            }
        }

        return null;
    }
}
