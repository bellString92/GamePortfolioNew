using System;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    [SerializeField] private int maxCount = 9;
    public TMPro.TMP_InputField count;
    public TMPro.TMP_Text desc;
    public StuffObject stuffMenu = StuffObject.None;
    public TableObject tableMenu = TableObject.None;
    public GameObject placeObj;

    private void Start()
    {
        string menuDesc = "";
        if (!stuffMenu.Equals(StuffObject.None))
        {
            Stuff obj = Resources.Load<Stuff>($"Prefabs/Stuff/{stuffMenu}");
            menuDesc += $"�̸� : {obj.myMenuDesc.menuName}\n";
            menuDesc += $"���� : {obj.myMenuDesc.kind}\n";
            menuDesc += $"�ܰ� : {obj.myMenuDesc.cost}";
        }
        else if (!tableMenu.Equals(TableObject.None))
        {
            Table obj = Resources.Load<Table>($"Prefabs/{tableMenu}");
            menuDesc += $"�̸� : {obj.myMenuDesc.menuName}\n";
            menuDesc += $"���� : {obj.myMenuDesc.kind}\n";
            menuDesc += $"�ܰ� : {obj.myMenuDesc.cost}";
        }

        desc.text = menuDesc;

    }

    public void OnCountPlus()
    {
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt < maxCount) cnt++;
            count.text = cnt.ToString();
        }
    }

    public void OnCountMinus()
    {
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt > 0) cnt--;
            count.text = cnt.ToString();
        }
    }

    public void OnChangeCount()
    {
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt < 0) cnt = 0;
            else if (cnt > maxCount) cnt = maxCount;
            count.text = cnt.ToString();
        }
    }

    public void OnBuy()
    {
        string msg = "";
        if (int.TryParse(count.text, out int cnt))
        {
            int result = 0;

            BuyPlace buy = placeObj.GetComponent<BuyPlace>();
            result = buy.GetPlaceRemainCount();

            if (result < cnt)
            {
                msg = $"���� ������ ����ּ���.\n �ִ� ���� ���� ���� : {result}";
                // �ӽ�
                Debug.Log(msg);
                return;
            }

            if (!stuffMenu.Equals(StuffObject.None))
            {
                buy.OnMenuBuyStuff(stuffMenu, cnt);
            }
            else if (!tableMenu.Equals(TableObject.None))
            {
                buy.OnMenuBuyTable(tableMenu, cnt);
            }
            
            msg = "���Ű� �Ϸ�Ǿ����ϴ�.";
            // �ӽ�
            Debug.Log(msg);
        }
    }
}
