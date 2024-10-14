using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TMPro.TMP_Text hour;
    public TMPro.TMP_Text min;
    private Coroutine cor;
    public OpenStore openStore;
    private Color resetColor;

    // Start is called before the first frame update
    void Start()
    {
        resetColor = new Color(10/255.0f, 100/255.0f, 0);
        ResetClock();
    }

    public void ResetClock()
    {
        hour.text = "08";
        min.text = "00";
        foreach (TMPro.TMP_Text text in hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            text.color = resetColor;
        }
        openStore.OpenedStore();
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
                if (hour.text.Equals("23"))
                {
                    hour.text = "00";
                }
                else
                {
                    hour.text = string.Format("{0:D2}", int.Parse(hour.text) + 1);
                }

                if (hour.text.Equals("22"))
                {
                    openStore.ClosedStore();
                    foreach (TMPro.TMP_Text text in hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
                    {
                        text.color = new Color(255 / 255.0f, 100 / 255.0f, 0);
                    }
                }
                else if (hour.text.Equals("02"))
                {
                    foreach (TMPro.TMP_Text text in hour.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>())
                    {
                        text.color = Color.red;
                    }
                }
            }
            else
            {
                this.min.text = string.Format("{0:D2}", int.Parse(this.min.text) + 1);
            }
        }
        /*
        yield return new WaitForSeconds(5.0f);
        ResetClock();
        */
    }
}
