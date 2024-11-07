using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LifterStaffState
{
    None, FindBox, Lift, Display, Put, Trash
}

public class LifterStaff : Staff
{
    public Transform boxPlace1;
    public Transform boxPlace2;
    public Coroutine findBoxCor = null;
    public Coroutine displayBoxCor = null;
    public Box takeBox = null;
    public Slots displaySlots;
    public LifterStaffState myLifterState = LifterStaffState.None;
    public Transform baseObj;
    public Transform garbageObj;

    void ChangeMyLifterState(LifterStaffState state)
    {
        myLifterState = state;
        switch (myLifterState) {
            case LifterStaffState.None:
                break;
            case LifterStaffState.FindBox:
                break;
            case LifterStaffState.Lift:
                ChangeMyActState(StaffActState.Walk);
                break;
            case LifterStaffState.Display:
                break;
            case LifterStaffState.Put:
                break;
            case LifterStaffState.Trash:
                break;
        }
    }

    void LifterStateProcess()
    {
        switch (myLifterState)
        {
            case LifterStaffState.None:
                ChangeMyLifterState(LifterStaffState.FindBox);
                break;
            case LifterStaffState.FindBox:
                if (takeBox == null && findBoxCor == null)
                    findBoxCor = StartCoroutine(FindBoxCor());
                else if (takeBox != null)
                {
                    if (findBoxCor != null)
                    {
                        StopCoroutine(findBoxCor);
                        findBoxCor = null;
                    }

                    if (onceChk)
                    {
                        onceChk = false;
                        ChangeMyActState(StaffActState.Walk);
                        purpose = takeBox.transform;
                        isLookRot = true;
                    }
                }
                break;
            case LifterStaffState.Lift:
                if (purpose == null)
                {
                    if (displaySlots != null)
                    {
                        ChangeMyActState(StaffActState.Walk);
                        purpose = displaySlots.transform.parent.GetComponent<Table>().staffPos;
                        isLookRot = true;
                    }
                }
                else
                {
                    if (onceChk)
                    {
                        onceChk = false;
                        ChangeMyLifterState(LifterStaffState.Display);
                        
                    }
                }
                break;
            case LifterStaffState.Display:
                if (displayBoxCor == null)
                {
                    ChangeMyActState(StaffActState.Idle);
                    takeBox.slots = displaySlots.transform.GetChild(0);
                    displayBoxCor = StartCoroutine(DisplayBoxCor());
                }
                break;
            case LifterStaffState.Put:
                if (onceChk)
                {
                    onceChk = false;
                    purpose = boxPlace2;
                    isLookRot = true;
                    ChangeMyActState(StaffActState.Walk);
                }
                break;
            case LifterStaffState.Trash:
                if (onceChk)
                {
                    onceChk = false;
                    purpose = garbageObj;
                    isLookRot = true;
                    ChangeMyActState(StaffActState.Walk);
                }
                break;

        }
    }


    protected override void WorkStateProcess()
    {
        if (myWorkState.Equals(StaffWorkState.Work))
        {
            LifterStateProcess();
        }
        else
        {
            base.WorkStateProcess();
        }
    }

    public void StopWalk()
    {
        ChangeMyActState(StaffActState.Idle);
        purpose = null;
        isLookRot = false;
        isMovePos = false;
        isPurposeRot = false;
    }

    public void LiftWork()
    {
        myAnim.SetBool("IsBox", true);
        myAnim.SetTrigger("OnBox");
        ChangeMyLifterState(LifterStaffState.Lift);
    }

    public void PutBox()
    {
        myAnim.SetBool("IsBox", false);
        takeBox = null;
        ChangeMyLifterState(LifterStaffState.FindBox);
    }

    IEnumerator FindBoxCor()
    {
        bool find = false;
        Box tmpBox = null;
        Slots tmpSlots = null;
        while (!find)
        {
            //foreach (Transform t in boxPlace1)
            for (int i = boxPlace1.childCount-1; i >= 0; i--)
            {
                Transform t = boxPlace1.GetChild(i);
                if (t.childCount > 0)
                {
                    //foreach (Box box in t.GetComponentsInChildren<Box>())
                    for (int j = t.childCount-1; j >= 0; j--)
                    {
                        Box box = t.GetChild(j).GetComponent<Box>();
                        foreach (Transform b in baseObj)
                        {
                            if (b.childCount > 0)
                            {
                                foreach (Slots s in b.GetComponentsInChildren<Slots>())
                                {
                                    if (s.OnDisplayCheck(box.stuff))
                                    {
                                        tmpSlots = s;
                                        tmpBox = box;
                                        find = true;
                                        break;
                                    }
                                    yield return null;
                                }
                            }
                            if (find) break;
                            yield return null;
                        }
                        if (find) break;
                        yield return null;
                    }
                }
                if (find) break;
                yield return null;
            }

            if (!find && boxPlace2.childCount > 0)
            {
                foreach (Transform t in boxPlace2)
                {
                    if (t.childCount > 0)
                    {
                        tmpBox = t.GetComponent<Box>();
                        foreach (Transform b in baseObj)
                        {
                            if (b.childCount > 0)
                            {
                                foreach (Slots s in b.GetComponentsInChildren<Slots>())
                                {
                                    if (s.OnDisplayCheck(tmpBox.stuff))
                                    {
                                        tmpSlots = s;
                                        find = true;
                                        break;
                                    }
                                    yield return null;
                                }
                            }
                            if (find) break;
                            yield return null;
                        }
                    }
                    if (find) break;
                    yield return null;
                }
                
            }
        }

        takeBox = tmpBox;
        displaySlots = tmpSlots;
        onceChk = true;
    }

    IEnumerator DisplayBoxCor()
    {
        while (!takeBox.IsEmpty() && displaySlots.OnDisplayCheck(takeBox.stuff))
        {
            yield return new WaitForSeconds(1.0f);
            takeBox.OnDisplay();
        }

        Debug.Log("¹èÄ¡³¡");
        purpose = null;
        displaySlots = null;
        onceChk = true;
        if (takeBox.IsEmpty())
        {
            ChangeMyLifterState(LifterStaffState.Trash);
            displayBoxCor = null;
        }
        else
        {
            StartCoroutine(FindSlotsCor());
        }
    }

    public void OnDrop(Box box)
    {
        if (box == takeBox)
        {
            StopWalk();
            takeBox.OnDrop(transform);
            LiftWork();
        }
    }

    IEnumerator FindSlotsCor()
    {
        Slots tmpSlots = null;
        bool find = false;
        float timeLimit = 10.0f;
        while (timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            foreach (Transform b in baseObj)
            {
                if (b.childCount > 0)
                {
                    foreach (Slots s in b.GetComponentsInChildren<Slots>())
                    {
                        if (s.OnDisplayCheck(takeBox.stuff))
                        {
                            tmpSlots = s;
                            find = true;
                            break;
                        }
                    }
                }
                if (find) break;
            }
            if (find) break;
            yield return null;
        }

        if (find)
        {
            displaySlots = tmpSlots;
            if (Vector3.Distance(transform.position, tmpSlots.transform.position) > 0.1f)
            {
                ChangeMyLifterState(LifterStaffState.Lift);
                onceChk = false;
            }
        }
        else ChangeMyLifterState(LifterStaffState.Put);
        displayBoxCor = null;
    }

    public void DestroyBox()
    {
        StopWalk();
        takeBox.OnDestroyBox();
        PutBox();
    }

    public void OnPut()
    {
        StopWalk();
        takeBox.OnPut(boxPlace2, this);
        PutBox();
    }
}
