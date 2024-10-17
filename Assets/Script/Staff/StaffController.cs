using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public enum StaffType
{
    Cashier, Lifter
}

public class StaffController : MonoBehaviour
{
    public static StaffController Instance;

    private bool cashierHire = false;
    private bool lifterHire = false;
    private bool cashierWork = false;
    private bool lifterWork = false;

    public Transform staffPos;
    public Transform staffMovingPos;
    public Transform sellPointPos;
    public Cashier cashier;

    CashierStaff cashierStaff;
    LifterStaff lifterStaff;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeStaffWork(StaffType staffType, bool hire)
    {
        switch (staffType)
        {
            case StaffType.Cashier:
                cashierHire = hire;
                if (Clock.Instance.GetClockTime() < 900)
                {
                    cashierWork = hire;
                }
                break;
            case StaffType.Lifter:
                lifterHire = hire;
                if (Clock.Instance.GetClockTime() < 900)
                {
                    lifterWork = hire;
                }
                break;
        }
    }

    public void ResetDay()
    {
        ChangeStaffWork(StaffType.Cashier, cashierHire);
        ChangeStaffWork(StaffType.Lifter, lifterHire);
    }

    public void StartWork()
    {
        if (cashierWork)
        {
            CreateStaff(StaffType.Cashier);
        }

        if (lifterWork)
        {
            CreateStaff(StaffType.Lifter);
        }
    }

    public void EndWork()
    {
        if (cashierWork)
        {
            cashierStaff.EndWork();
            cashierStaff.purpose = staffMovingPos.GetChild(0).GetChild(1).GetChild(0);
        }

        if (lifterWork)
        {
            lifterStaff.EndWork();
            lifterStaff.purpose = staffMovingPos.GetChild(1).GetChild(3).GetChild(0);
        }
    }

    void CreateStaff(StaffType staffType)
    {
        GameObject staff = ObjectPool.Instance.Instantiate<StaffType>(Resources.Load($"Prefabs/Staff/{staffType}") as GameObject, staffPos);
        staff.name = staffType.ToString();
        if (staffType.Equals(StaffType.Cashier))
        {
            cashierStaff = staff.GetComponent<CashierStaff>();
            cashierStaff.purpose = staffMovingPos.GetChild(0).GetChild(0).GetChild(0);
            cashierStaff.ChangeMyActState(StaffActState.Walk);
            cashierStaff.ChangeMyWorkState(StaffWorkState.Start);
            cashierStaff.sellPointPos = sellPointPos;
            cashierStaff.cashier = cashier;
        }
        else if (staffType.Equals(StaffType.Lifter))
        {
            lifterStaff = staff.GetComponent<LifterStaff>();
            lifterStaff.purpose = staffMovingPos.GetChild(1).GetChild(0).GetChild(0);
            lifterStaff.ChangeMyActState(StaffActState.Walk);
            lifterStaff.ChangeMyWorkState(StaffWorkState.Start);
        }
    }

    public int GetStaffPay()
    {
        int result = 0;
        if (cashierWork) result += 10000;
        if (lifterWork) result += 10000;

        return result;
    }

    public void OnDestroyStaff(StaffType staff)
    {
        if (staff.Equals(StaffType.Cashier))
            cashierStaff = null;
        else if (staff.Equals(StaffType.Lifter))
            lifterStaff = null;
    }
}
