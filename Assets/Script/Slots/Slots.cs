using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public StuffObject myStuff;
    public Transform[] slotArr = null;
    public int count = 0;
    public bool countChk = true;
    public GameObject priceObj;
    public int price = 0;

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

    private void ChangeMyStuff(StuffObject stuffObject, int price)
    {
        myStuff = stuffObject;
        this.price = price;
        priceObj.GetComponentInChildren<TMPro.TMP_Text>().text = Global.Comma(price) + "원";
        priceObj.SetActive(true);
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
        if (count == 0) ChangeMyStuff(t.GetComponent<Stuff>().stuff, Global.GetStuff(t.GetComponent<Stuff>().stuff).myStuffDesc.price);
        t.SetParent(slotArr[count++]);
        t.localScale = new Vector3(2, 40, 1);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.gameObject.SetActive(true);
        if (count >= 10) countChk = false;
    }

    public void PutStuff(Transform t)
    {
        // 음식 집기
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
                    s.GetComponent<Slots>().GetComponentInChildren<TMPro.TMP_Text>().text = Global.Comma(price) + "원";
                }
            }
        }
    }
}
