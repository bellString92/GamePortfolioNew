using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenStore : MonoBehaviour
{
    public GameObject openObj;
    public GameObject closeObj;
    public bool isOpen = false;
    public bool closedDay = true;

    private void Start()
    {
        ChangeOpen();
    }

    public void OnOpenToggle()
    {
        if (closedDay) return;

        isOpen = !isOpen;
        ChangeOpen();
    }

    public void ClosedStore()
    {
        isOpen = false;
        closedDay = true;
        ChangeOpen();
    }

    public void OpenedStore()
    {
        closedDay = false;
    }

    private void ChangeOpen()
    {
        openObj.SetActive(isOpen);
        closeObj.SetActive(!isOpen);
        if (isOpen)
        {
            GuestController.Instance.OnCreateGuest();
        }
        else
        {
            GuestController.Instance.StopGuest();
        }
    }
}
