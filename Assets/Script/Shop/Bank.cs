using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank instance = null;

    public static int loan1 = 0;
    private int interest1 = 5;
    public static int loan2 = 0;
    private int interest2 = 10;
    public static int loan3 = 0;
    private int interest3 = 15;

    public Transform loanObj1;
    public Transform loanObj2;
    public Transform loanObj3;

    public TMPro.TMP_Text loanDesc1;
    public TMPro.TMP_Text loanDesc2;
    public TMPro.TMP_Text loanDesc3;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        loanObj1.GetChild(0).gameObject.SetActive(true);
        loanObj1.GetChild(1).gameObject.SetActive(false);
        loanObj2.GetChild(0).gameObject.SetActive(true);
        loanObj2.GetChild(1).gameObject.SetActive(false);
        loanObj3.GetChild(0).gameObject.SetActive(true);
        loanObj3.GetChild(1).gameObject.SetActive(false);
    }

    public void LoanGold(int loan)
    {
        Global.Gold += loan;
        Gold.Instance.OnChangeGold();

        switch (loan)
        {
            case 500000:
                loan1 += loan;
                loanObj1.GetChild(0).gameObject.SetActive(false);
                loanObj1.GetChild(1).gameObject.SetActive(true);
                loanDesc1.text = $"금액 : 500,000원\r\n이자 : 5%\r\n남은돈 : {Global.Comma(loan1)}원";
                break;
            case 2000000:
                loan2 += loan;
                loanObj2.GetChild(0).gameObject.SetActive(false);
                loanObj2.GetChild(1).gameObject.SetActive(true);
                loanDesc2.text = $"금액 : 2,000,000원\r\n이자 : 10%\r\n남은돈 : {Global.Comma(loan2)}원";
                break;
            case 5000000:
                loan3 += loan;
                loanObj3.GetChild(0).gameObject.SetActive(false);
                loanObj3.GetChild(1).gameObject.SetActive(true);
                loanDesc3.text = $"금액 : 5,000,000원\r\n이자 : 15%\r\n남은돈 : {Global.Comma(loan3)}원";
                break;
        }
    }

    public void RepayGold(int loanNum = 1)
    {
        if (Global.Gold <= 0)
        {
            Debug.Log("보유한 골드가 부족합니다.");
            return;
        }
        switch (loanNum)
        {
            case 1:
                if (Global.Gold >= loan1)
                {
                    Global.Gold -= loan1;
                    loan1 = 0;
                    loanObj1.GetChild(0).gameObject.SetActive(true);
                    loanObj1.GetChild(1).gameObject.SetActive(false);
                    loanDesc1.text = $"금액 : 500,000원\r\n이자 : 5%";
                }
                else
                {
                    loan1 -= Global.Gold;
                    Global.Gold = 0;
                    loanDesc1.text = $"금액 : 500,000원\r\n이자 : 5%\r\n남은돈 : {Global.Comma(loan1)}원";
                }
                break;
            case 2:
                if (Global.Gold >= loan2)
                {
                    Global.Gold -= loan2;
                    loan2 = 0;
                    loanObj2.GetChild(0).gameObject.SetActive(true);
                    loanObj2.GetChild(1).gameObject.SetActive(false);
                    loanDesc2.text = $"금액 : 2,000,000원\r\n이자 : 10%";
                }
                else
                {
                    loan2 -= Global.Gold;
                    Global.Gold = 0;
                    loanDesc2.text = $"금액 : 2,000,000원\r\n이자 : 10%\r\n남은돈 : {Global.Comma(loan2)}원";
                }
                break;
            case 3:
                if (Global.Gold >= loan3)
                {
                    Global.Gold -= loan3;
                    loan3 = 0;
                    loanObj3.GetChild(0).gameObject.SetActive(true);
                    loanObj3.GetChild(1).gameObject.SetActive(false);
                    loanDesc3.text = $"금액 : 5,000,000원\r\n이자 : 15%";
                }
                else
                {
                    loan3 -= Global.Gold;
                    Global.Gold = 0;
                    loanDesc3.text = $"금액 : 5,000,000원\r\n이자 : 15%\r\n남은돈 : {Global.Comma(loan3)}원";
                }
                break;
        }
        Gold.Instance.OnChangeGold();
    }

    public int GetInterest()
    {
        int interest = 0;
        if (loan1 > 0)
        {
            interest += loan1 * interest1 / 100;
        }

        if (loan2 > 0)
        {
            interest += loan2 * interest2 / 100;
        }

        if (loan3 > 0)
        {
            interest += loan3 * interest3 / 100;
        }

        return interest;
    }
}
