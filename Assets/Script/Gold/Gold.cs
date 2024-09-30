using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Gold : MonoBehaviour
{
    //10만원
    private static bool chk = false;
    Coroutine cor = null;

    public static Gold Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnChangeGold();
    }

    private void Update()
    {
        // 10만원
        if (chk)
        {
            OnChangeGold();
            chk = false;
        }
    }

    public void OnChangeGold()
    {
        if (cor == null)
        {
            cor = StartCoroutine(ChangingGold());
        }
    }

    IEnumerator ChangingGold()
    {
        int oriGold = Global.UnComma(transform.GetComponent<TMPro.TMP_Text>().text);
        
        while (oriGold < Global.Gold - 1000)
        {
            transform.GetComponent<TMPro.TMP_Text>().text = Global.Comma(oriGold += 1000);
            yield return null;
        }

        while (oriGold > Global.Gold + 1000)
        {
            transform.GetComponent<TMPro.TMP_Text>().text = Global.Comma(oriGold -= 1000);
            yield return null;
        }

        transform.GetComponent<TMPro.TMP_Text>().text = Global.GoldStr;

        cor = null;
    }

    [MenuItem("Tools/ShowMeTheMoney")]
    public static void ShowMeTheMoney()
    {
        Global.Gold += 100000;
        chk = true;
    }
}
