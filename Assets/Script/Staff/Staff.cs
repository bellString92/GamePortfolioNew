using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StaffActState
{
    Idle, Walk
}

public enum StaffWorkState
{
    Start, Work, End
}

public class Staff : AnimatorProperty
{
    public StaffType myStaffType;

    public StaffActState myActState;
    public StaffWorkState myWorkState;
    public bool isLookRot = false;
    public bool isMovePos = false;
    public bool isPurposeRot = false;
    public bool onceChk = false;
    public Transform purpose;

    private float rotSpeed = 5.0f;
    private float moveSpeed = 3.0f;


    public void ChangeMyActState(StaffActState actState)
    {
        myActState = actState;

        switch (actState)
        {
            case StaffActState.Idle:
                myAnim.SetBool("IsWalk", false);
                break;
            case StaffActState.Walk:
                myAnim.SetBool("IsWalk", true);
                myAnim.SetTrigger("OnWalk");
                break;
        }
    }

    public void ChangeMyWorkState(StaffWorkState workState)
    {
        myWorkState = workState;

        switch (workState)
        {
            case StaffWorkState.Start:
                isLookRot = true;
                break;
            case StaffWorkState.End:
                isLookRot = true;
                break;
        }
    }

    protected virtual void WorkStateProcess()
    {
        switch (myWorkState)
        {
            case StaffWorkState.Start:
                if (onceChk)
                {
                    onceChk = false;
                    if (purpose.GetSiblingIndex() < purpose.parent.childCount - 1)
                    {
                        purpose = purpose.parent.GetChild(purpose.GetSiblingIndex() + 1);
                        ChangeMyActState(StaffActState.Walk);
                        isLookRot = true;
                    }
                    else
                    {
                        ChangeMyActState(StaffActState.Idle);
                        ChangeMyWorkState(StaffWorkState.Work);
                    }
                }
                break;
            case StaffWorkState.End:
                if (onceChk)
                {
                    onceChk = false;
                    if (purpose.GetSiblingIndex() < purpose.parent.childCount - 1)
                    {
                        purpose = purpose.parent.GetChild(purpose.GetSiblingIndex() + 1);
                        ChangeMyActState(StaffActState.Walk);
                        isLookRot = true;
                    }
                    else
                    {
                        StaffController.Instance.OnDestroyStaff(myStaffType);
                        ObjectPool.Instance.Release<StaffType>(gameObject);
                        ResetStaff();
                    }
                }
                break;
        }
    }

    public void EndWork()
    {
        ChangeMyActState(StaffActState.Walk);
        ChangeMyWorkState(StaffWorkState.End);
    }

    protected void Update()
    {
        WorkStateProcess();

        if (isLookRot)
        {
            Vector3 purposePos = purpose.position - transform.position;
            purposePos.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(purposePos);

            if (Quaternion.Angle(transform.rotation, targetRotation) > 1.0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = targetRotation;
                isLookRot = false;
                isMovePos = true;
            }
        }
        else if (isMovePos)
        {

            if (Vector3.Distance(transform.position, purpose.position) > 0.1f)
            {

                Vector3 newPurpose = new Vector3(purpose.position.x, 0, purpose.position.z);
                transform.position = Vector3.MoveTowards(transform.position, newPurpose, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = purpose.position;
                isMovePos = false;
                isPurposeRot = true;
            }
        }
        else if (isPurposeRot)
        {
            float angle = Quaternion.Angle(transform.rotation, purpose.rotation);
            if (angle > 1.0f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, purpose.rotation, rotSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = purpose.rotation;
                isPurposeRot = false;
                onceChk = true;
            }
        }
        
    }

    protected virtual void ResetStaff()
    {
        myActState = StaffActState.Idle;
        myWorkState = StaffWorkState.Start;
        isLookRot = false;
        isMovePos = false;
        isPurposeRot = false;
        onceChk = false;
        purpose = null;
}
}
