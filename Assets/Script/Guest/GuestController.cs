using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuestCharacter
{
    nonPBR, Kachujin
}

public enum GuestState
{
    Join, Wait, Get, Buy, Out, Steal
}

public class GuestController : MonoBehaviour
{
    public int count = 0;
    public int maxCount = 10;

}
