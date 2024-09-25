using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPlace : MonoBehaviour
{
    [SerializeField] private int maxCount = 0;
    private int count = 0;

    public void OnMenuBuyStuff(StuffObject so, int count)
    {
        bool chk = false;
        Debug.Log($"{so}를 {count}만큼 구매.");
        for (int i = 0; i < count; i++)
        {
            chk = false;
            GameObject obj = Instantiate(Resources.Load("Prefabs/FoodBox") as GameObject);
            obj.name = "FoodBox";
            foreach (Transform t in transform)
            {
                if (t.childCount >= 5) continue;
                obj.transform.SetParent(t);
                obj.transform.localPosition = new Vector3(0, t.childCount, 0);
                Debug.Log(t.childCount);
                obj.GetComponent<Box>().OnCreateBox(so);
                chk = true;
                break;
            }
            if (!chk) break;
        }
    }

    public void OnMenuBuyTable(TableObject to, int count)
    {
        bool chk = false;
        Debug.Log($"{to}를 {count}만큼 구매.");
        for (int i = 0; i < count; i++)
        {
            chk = false;
            GameObject obj = Instantiate(Resources.Load($"Prefabs/{to}") as GameObject);
            obj.name = to.ToString();
            foreach (Transform t in transform)
            {
                if (t.childCount > 0) continue;
                obj.transform.SetParent(t);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.GetComponent<Table>().tableObject = to;
                chk = true;
                break;
            }
            if (!chk) break;
        }
    }
    public int GetPlaceRemainCount()
    {
        SetCount();
        return maxCount - count;
    }
    
    private void SetCount()
    {
        count = 0;
        foreach (Transform t in transform)
        {
            count += t.childCount;
        }
    }
}
