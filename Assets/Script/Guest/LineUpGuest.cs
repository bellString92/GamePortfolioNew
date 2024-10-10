using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpGuest : MonoBehaviour
{
    public Queue<Guest> guests = new();
    public int maxCount = 10;
    public int fillCount = 0;

    public void EnqueueGuest(Guest guest)
    {
        guests.Enqueue(guest);
        fillCount = guests.Count;
    }

    public Guest DequeueGuest()
    {
        fillCount = guests.Count-1;
        return guests.Dequeue();
    }

    public bool IsEmpty()
    {
        return fillCount == 0;
    }

    public bool IsFull()
    {
        return maxCount <= fillCount;
    }

    public int GetCurPoint()
    {
        return fillCount;
    }

    public Queue<Guest> GetAllGuest()
    {
        return new Queue<Guest>(new Queue<Guest>(guests));
    }
}
