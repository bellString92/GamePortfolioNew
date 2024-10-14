using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
        {
            other.gameObject.GetComponent<Player>().onBed = true;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
            other.gameObject.GetComponent<Player>().onBed = false;
    }
}
