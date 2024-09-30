using System;
using Unity.VisualScripting;
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
    public TMPro.TMP_Text price;
    public int cost;

    private void Start()
    {
        string menuDesc = "";
        if (!stuffMenu.Equals(StuffObject.None))
        {
            Stuff obj = Global.GetStuff(stuffMenu);
            menuDesc += $"�̸� : {obj.myStuffDesc.menuName}\n";
            menuDesc += $"���� : {obj.myStuffDesc.kind}\n";
            menuDesc += $"���� : 20��\n";
            menuDesc += $"�ܰ�\n{Global.Comma(obj.myStuffDesc.cost)}��";
            this.cost = obj.myStuffDesc.cost;
        }
        else if (!tableMenu.Equals(TableObject.None))
        {
            Table obj = Resources.Load<Table>($"Prefabs/{tableMenu}");
            menuDesc += $"�̸� : {obj.myStuffDesc.menuName}\n";
            menuDesc += $"���� : {obj.myStuffDesc.kind}\n";
            menuDesc += $"�ܰ�\n{Global.Comma(obj.myStuffDesc.cost)}��";
            this.cost = obj.myStuffDesc.cost;
        }

        desc.text = menuDesc;
    }

    public void OnCountPlus()
    {
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt < maxCount) cnt++;
            count.text = cnt.ToString();
            UpdatePrice(!stuffMenu.Equals(StuffObject.None));
        }
    }

    public void OnCountMinus()
    {
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt > 0) cnt--;
            count.text = cnt.ToString();
            UpdatePrice(!stuffMenu.Equals(StuffObject.None));
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
        UpdatePrice(!stuffMenu.Equals(StuffObject.None));
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

            Global.Gold -= Global.UnComma(price.text.Substring(0, price.text.Length - 1));
            Gold.Instance.OnChangeGold();

            msg = "���Ű� �Ϸ�Ǿ����ϴ�.";

            count.text = "0";

            // �ӽ�
            Debug.Log(msg);
        }
    }

    void UpdatePrice(bool isStuff)
    {
        int amount = 1;
        if (isStuff) amount = 20;
        if (int.TryParse(count.text, out int cnt))
        {
            if (cnt <= 0) price.text = "0��";
            else price.text = Global.Comma(cnt * cost * amount) + "��";
        }
    }
}
