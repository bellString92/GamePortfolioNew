using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;

public class Statistics
{
    public int guest = 0; // 입장 게스트 수
    public int buyGuest = 0; // 판매 게스트 수

    public int saleGold = 0; // 매출
    public int profitGold = 0; // 순이익

    public int unpaid = 0; // 미납 횟수
}

public class Global
{
    private static int _gold = 0;
    private static Dictionary<StuffObject, Stuff> _stuff = new Dictionary<StuffObject, Stuff>();
    public static Statistics statistics = new Statistics();

    public static void ResetDay(Player player)
    {
        statistics.guest = 0;
        statistics.buyGuest = 0;
        statistics.saleGold = 0;
        statistics.profitGold = 0;

        player.HideInformation();
    }

    public static int Gold
    {
        get => _gold;
        set => _gold = value;
    }

    public static string GoldStr
    {
        get => Comma(_gold);
    }

    public static Stuff GetStuff(StuffObject so)
    {
        if (so != StuffObject.None && _stuff.ContainsKey(so))
            return _stuff[so];
        else return null;
    }

    public static void SetStuff(StuffObject so, Stuff stuff)
    {
        if (so != StuffObject.None && stuff != null)
            _stuff[so] = stuff;
    }

    public static void SetStuffPrice(StuffObject so, int price)
    {
        if (so != StuffObject.None && _stuff.ContainsKey(so))
            _stuff[so].myStuffDesc.price = price;
    }

    public static string Comma(int data)
    {
        if (data == 0) return "0";
        return string.Format("{0:#,###}", data);
    }

    public static int UnComma(string str)
    {
        return int.Parse(str.Replace(",", ""));
    }
}
