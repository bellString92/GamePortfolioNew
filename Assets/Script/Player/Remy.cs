using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remy : MonoBehaviour
{
    public Player player;

    public void EndDay()
    {
        player.EndDay();
    }

    public void MovePlayer()
    {
        player.MovePlayer();
    }
}
