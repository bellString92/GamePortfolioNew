using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifterStaff : Staff
{
    protected override void WorkStateProcess()
    {
        if (myWorkState.Equals(StaffWorkState.Work))
        {

        }
        else
        {
            base.WorkStateProcess();
        }
    }
}
