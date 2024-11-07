using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<LifterStaff>() != null && other.GetComponent<LifterStaff>().myWorkState.Equals(StaffWorkState.Work))
        {
            other.GetComponent<LifterStaff>().DestroyBox();
        }
    }
}
