using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public LayerMask targetMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & targetMask) != 0)
        {
            other.GetComponent<Player>().doorTriggerChk = other.GetComponent<Player>().onDoorTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & targetMask) != 0)
        {
            other.GetComponent<Player>().onDoorTrigger = false;
            other.GetComponent<Player>().doorTriggerChk = true;
        }
    }

}
