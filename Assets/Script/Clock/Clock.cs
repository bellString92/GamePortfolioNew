using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TMPro.TMP_Text hour;
    public TMPro.TMP_Text min;
    private Coroutine cor;
    private Color resetColor;
    public static Clock Instance = null;

    public OpenStore openStore;
    public StaffController staffController;


    private void Awake()
    {
        Instance = this;
    }

    public void StopClock()
    {
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetClock();
    }

    public void ResetClock()
    {
        resetColor = new Color(10 / 255.0f, 100 / 255.0f, 0);

        hour.text = "08";
        min.text = "00";
        foreach (TMPro.TMP_Text text in hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            text.color = resetColor;
        }
        openStore.OpenedStore();
        staffController.ResetDay();
    }

    public void StartClock()
    {
        cor = StartCoroutine(FlowClock(1.0f));
    }

    IEnumerator FlowClock(float min)
    {
        while (!hour.text.Equals("03"))
        {
            yield return new WaitForSeconds(1 * min / 14.0f);
            if (this.min.text.Equals("59"))
            {
                this.min.text = "00";
                string hour = string.Format("{0:D2}", int.Parse(this.hour.text) + 1);

                switch (hour)
                {
                    case "24":
                        hour = "00";
                        break;
                    case "22":
                        openStore.ClosedStore();
                        foreach (TMPro.TMP_Text text in this.hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
                        {
                            text.color = new Color(255 / 255.0f, 100 / 255.0f, 0);
                        }
                        break;
                    case "03":
                        foreach (TMPro.TMP_Text text in this.hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
                        {
                            text.color = Color.red;
                        }
                        break;
                    case "09":
                        staffController.StartWork();
                        break;
                    case "18":
                        staffController.EndWork();
                        break;
                }

                this.hour.text = hour;
            }
            else
            {
                this.min.text = string.Format("{0:D2}", int.Parse(this.min.text) + 1);
            }
        }
    }

    public int GetClockTime()
    {
        return int.Parse(hour.text + min.text);
    }
}
