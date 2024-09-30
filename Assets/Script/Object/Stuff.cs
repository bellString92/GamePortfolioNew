using System;
using UnityEngine;

[Serializable]
public struct StuffDesc
{
    public int cost;
    public int price;
    public string menuName;
    public string kind;
}

public enum StuffObject
{
    None = 0,
    Burger = 1, Donut, Snack,
    Coke = 11, Juice, Yogurt,
}


public class Stuff : MonoBehaviour
{
    public StuffObject stuff;
    public StuffDesc myStuffDesc;
}
