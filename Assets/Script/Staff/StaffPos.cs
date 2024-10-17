using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffPos : MonoBehaviour
{
    public Transform staffMovingPos;
    public Transform sellPointPos;
    public StaffController staffController;
    public Cashier cashier;

    private void Start()
    {
        staffController.staffPos = transform;
        staffController.staffMovingPos = staffMovingPos;
        staffController.sellPointPos = sellPointPos;
        staffController.cashier = cashier;
    }
}
