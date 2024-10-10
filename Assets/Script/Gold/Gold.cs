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

    private TMPro.TMP_Text goldText;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        goldText = transform.GetComponent<TMPro.TMP_Text>();
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
        if (cor != null)
        {
            StopCoroutine(cor);
        }
        cor = StartCoroutine(ChangingGold());
    }

    IEnumerator ChangingGold(int changeGold = 1000000)
    {
        int oriGold = Global.UnComma(goldText.text);


        while (oriGold < Global.Gold - 1)
        {
            while (changeGold > 1 && oriGold + changeGold >= Global.Gold)
            {
                changeGold /= 10;
            }
            goldText.text = Global.Comma(oriGold += changeGold);
            yield return new WaitForSeconds(0.02f);
        }

        while (oriGold > Global.Gold + 1)
        {
            while (changeGold > 1 && oriGold - changeGold <= Global.Gold)
            {
                changeGold /= 10;
            }
            goldText.text = Global.Comma(oriGold -= changeGold);
            yield return new WaitForSeconds(0.02f);
        }

        goldText.text = Global.Comma(Global.Gold);

        cor = null;
    }

    [MenuItem("Tools/ShowMeTheMoney")]
    public static void ShowMeTheMoney()
    {
        Global.Gold += 100000;
        chk = true;
    }
}
