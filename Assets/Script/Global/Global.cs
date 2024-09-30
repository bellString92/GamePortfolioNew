using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    private static int _gold = 0;
    private static Dictionary<StuffObject, Stuff> _stuff = new Dictionary<StuffObject, Stuff>();

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
