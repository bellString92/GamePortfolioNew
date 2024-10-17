using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hire : MonoBehaviour
{
    [SerializeField] private Transform cashier;
    [SerializeField] private Transform lifter;

    public StaffController staffController;

    public void CashierHire()
    {
        cashier.GetChild(0).gameObject.SetActive(false);
        cashier.GetChild(1).gameObject.SetActive(true);
        staffController.ChangeStaffWork(StaffType.Cashier, true);
    }

    public void CashierFire()
    {
        cashier.GetChild(0).gameObject.SetActive(true);
        cashier.GetChild(1).gameObject.SetActive(false);
        staffController.ChangeStaffWork(StaffType.Cashier, false);
    }

    public void LifterHire()
    {
        lifter.GetChild(0).gameObject.SetActive(false);
        lifter.GetChild(1).gameObject.SetActive(true);
        staffController.ChangeStaffWork(StaffType.Lifter, true);
    }

    public void LifterFire()
    {
        lifter.GetChild(0).gameObject.SetActive(true);
        lifter.GetChild(1).gameObject.SetActive(false);
        staffController.ChangeStaffWork(StaffType.Lifter, false);
    }
}
