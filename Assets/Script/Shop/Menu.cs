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
            menuDesc += $"이름 : {obj.myMenuDesc.menuName}\n";
            menuDesc += $"종류 : {obj.myMenuDesc.kind}\n";
            menuDesc += $"단가 : {obj.myMenuDesc.cost}";
        }
        else if (!tableMenu.Equals(TableObject.None))
        {
            Table obj = Resources.Load<Table>($"Prefabs/{tableMenu}");
            menuDesc += $"이름 : {obj.myMenuDesc.menuName}\n";
            menuDesc += $"종류 : {obj.myMenuDesc.kind}\n";
            menuDesc += $"단가 : {obj.myMenuDesc.cost}";
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
                msg = $"구매 공간을 비워주세요.\n 최대 구매 가능 갯수 : {result}";
                // 임시
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
            
            msg = "구매가 완료되었습니다.";
            // 임시
            Debug.Log(msg);
        }
    }
}
