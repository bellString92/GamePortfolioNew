using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    public Transform[] actOrder = new Transform[9];
    public Player player;
    private int interest = 0;
    private int staffPay = 0;
    public StaffController staffController;

    private float fastInformation = 0.5f;


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            fastInformation = 0.1f;
        }
    }

    public void StartInformation()
    {
        fastInformation = 0.5f;
        staffPay = staffController.GetStaffPay();

        if (Global.Gold >= staffPay)
        {
            Global.Gold -= staffPay;
        }
        else
        {
            staffPay = Global.Gold;
            Global.Gold = 0;
        }

        interest = Bank.instance.GetInterest();
        if (Global.Gold >= interest)
        {
            Global.Gold -= interest;
        }
        else
        {
            interest = Global.Gold;
            Global.Gold = 0;
            Global.statistics.unpaid++;
        }


        Gold.Instance.OnChangeGold();

        if (Global.statistics.unpaid >= 5)
        {
            actOrder[8].GetComponentInChildren<TMPro.TMP_Text>().text = "GameOver";
            actOrder[8].GetComponent<Button>().onClick.RemoveAllListeners();
            actOrder[8].GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });
        }
        else
        {
            actOrder[8].GetComponent<Button>().onClick.RemoveAllListeners();
            actOrder[8].GetComponent<Button>().onClick.AddListener(() =>
            {
                player.ResetDay();
                gameObject.SetActive(false);
                for (int i = 0; i < actOrder.Length; i++)
                {
                    switch (i)
                    {
                        case 1:
                        case 4:
                            {
                                foreach (Transform t in actOrder[i])
                                {
                                    t.gameObject.SetActive(false);
                                }
                            }
                            break;
                        case 7:
                            actOrder[i].GetChild(0).gameObject.SetActive(false);
                            break;
                        case 8:
                        default:
                            actOrder[i].gameObject.SetActive(false);
                            break;
                    }
                }
            });
        }


        Clock.Instance.ResetClock();


        gameObject.SetActive(true);
        StartCoroutine(InformationCor());
    }

    IEnumerator InformationCor()
    {
        for (int i = 0; i < actOrder.Length; i++)
        {
            switch (i)
            {
                case 1:
                case 4:
                    {
                        foreach (Transform t in actOrder[i])
                        {
                            int j = t.GetSiblingIndex();
                            if (i == 1)
                            {
                                if (j == 1)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"{Global.statistics.buyGuest}";
                                }
                                else if (j == 3)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"{Global.statistics.guest - Global.statistics.buyGuest}";
                                }
                                else if (j == 5)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"{Global.statistics.guest}";
                                }
                            }
                            else if (i == 4)
                            {
                                if (j == 1)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"+{Global.Comma(Global.statistics.saleGold)}원";
                                }
                                else if (j == 3)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"+{Global.Comma(Global.statistics.profitGold)}원";
                                }
                                else if (j == 5)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"-{Global.Comma(interest)}원";
                                }
                                else if (j == 7)
                                {
                                    t.GetComponent<TMPro.TMP_Text>().text = $"-{Global.Comma(staffPay)}원";
                                }
                            }
                            yield return new WaitForSeconds(fastInformation);
                            t.gameObject.SetActive(true);
                        }
                    }
                    break;
                case 7:
                    yield return new WaitForSeconds(fastInformation*2);
                    {
                        TMPro.TMP_Text text = actOrder[i].GetChild(0).GetComponent<TMPro.TMP_Text>();
                        Color c = text.color;
                        string tmp = "";
                        if (Global.Gold > 0)
                        {
                            c = new Color(0, 1, 0);
                            tmp = "+";
                        }
                        else if (Global.Gold < 0)
                        {
                            c = new Color(1, 0, 0);
                        }
                        else
                        {
                            c = new Color(1, 1, 1);
                        }
                        text.color = c;
                        text.text = $"{tmp}{Global.Comma(Global.Gold)}원";
                    }
                    actOrder[i].GetChild(0).gameObject.SetActive(true);
                    break;
                case 8:
                    yield return new WaitForSeconds(fastInformation*2);
                    actOrder[i].gameObject.SetActive(true);
                    break;
                default:
                    yield return new WaitForSeconds(fastInformation);
                    actOrder[i].gameObject.SetActive(true);
                    break;
            }
        }
    }
}
