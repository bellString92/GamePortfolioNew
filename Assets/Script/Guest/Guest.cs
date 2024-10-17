using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEditor;

public enum GuestActState
{
    Walk, Idle, Pick, Steal
}

public enum GuestCurrentState
{
    Start, Door, Join, Select, PickWait, Pick, BuyWait, Buy, Pay, Complete,
    End = 100, Exit, Destroy
}

public class Guest : AnimatorProperty
{
    public GuestActState myActState;
    public GuestCurrentState myCurrentState;
    public Transform purposeObj;
    public Transform purpose;
    public Transform myPurposeLineUp = null;

    private bool isLookRot = false;
    private bool isMovePos = false;
    private bool isPurposeRot = false;
    private float rotSpeed = 5.0f;
    private float moveSpeed = 3.0f;

    public Dictionary<StuffObject, int> hopeStuff = new Dictionary<StuffObject, int>();
    public Dictionary<StuffObject, int> takeStuff = new Dictionary<StuffObject, int>();

    public bool onceChk = false;

    private Slots putStuffSlot = null;
    private Transform putStuff = null;

    public Transform myBasket;

    private void Start()
    {
        if (myBasket == null)
        {
            myBasket = transform.GetChild(transform.childCount - 1);
        }
    }

    public void ChangeMyActState(GuestActState state)
    {
        if (myActState != state) myActState = state;
        switch (state)
        {
            case GuestActState.Idle:
                myAnim.SetBool("IsWalk", false);
                myAnim.SetBool("IsPick", false);
                myAnim.SetBool("IsSteal", false);
                break;
            case GuestActState.Pick:
                myAnim.SetBool("IsWalk", false);
                myAnim.SetBool("IsPick", true);
                myAnim.SetTrigger("OnPick");
                break;
            case GuestActState.Walk:
            case GuestActState.Steal:
                myAnim.SetBool($"Is{state}", true);
                myAnim.SetTrigger($"On{state}");
                isLookRot = true;
                break;
        }
    }

    void ActStateProcess()
    {
        switch (myActState)
        {
            case GuestActState.Idle:
                if (purpose == null)
                    ChangePurpose(GuestController.Instance.UpdateGuestPurpose(myCurrentState, this));
                else
                    ChangeMyActState(GuestActState.Walk);
                break;
            case GuestActState.Pick:
                break;
            case GuestActState.Walk:
                break;
            case GuestActState.Steal:
                break;
        }
    }

    public void ChangeMyCurrentState(GuestCurrentState currentState)
    {
        myCurrentState = currentState;
        switch (myCurrentState)
        {
            case GuestCurrentState.Pick:
            case GuestCurrentState.Buy:
                onceChk = true;
                break;
        }
    }

    void CurrentStateProcess()
    {
        switch (myCurrentState)
        {
            case GuestCurrentState.Door:
                if (onceChk)
                {
                    onceChk = false;
                    SelectStuff();
                }
                break;
            case GuestCurrentState.PickWait:
                if (onceChk && myPurposeLineUp != null && myPurposeLineUp.GetSiblingIndex() == 0)
                {
                    onceChk = false;
                    ChangeMyCurrentState(GuestCurrentState.Pick);
                    ChangeMyActState(GuestActState.Pick);
                }
                break;
            case GuestCurrentState.Pick:
                if (onceChk)
                {
                    onceChk = false;
                    
                    if (putStuffSlot == null)
                    {
                        foreach (Slots s in myPurposeLineUp.parent.GetComponent<BaseGuest>().mySlots.GetComponentsInChildren<Slots>())
                        {
                            if (hopeStuff.ContainsKey(s.myStuff) && hopeStuff[s.myStuff] > 0)
                            {
                                putStuffSlot = s;
                                break;
                            }
                        }
                    }

                    if (putStuffSlot != null)
                    {
                        putStuff = putStuffSlot.PutStuff(false);
                        if (putStuff != null)
                            ChangeMyActState(GuestActState.Pick);
                        else
                        {
                            putStuffSlot = null;
                            onceChk = true;
                        }
                    }
                    else
                    {
                        GuestController.Instance.EmptyBaseStuff(myPurposeLineUp.parent.GetComponent<BaseGuest>());
                    }
                }
                break;
            case GuestCurrentState.BuyWait:
                if (onceChk && myPurposeLineUp != null && myPurposeLineUp.GetSiblingIndex() == 0)
                {
                    onceChk = false;
                    int takeNum = 0;
                    foreach (KeyValuePair<StuffObject, int> take in takeStuff) takeNum += take.Value;
                    myPurposeLineUp.parent.GetComponent<CashierGuest>().takeStuffNum = takeNum;
                    ChangeMyCurrentState(GuestCurrentState.Buy);
                    ChangeMyActState(GuestActState.Idle);
                }
                break;
            case GuestCurrentState.Buy:
                if (onceChk)
                {
                    onceChk = false;
                    GuestController.Instance.SpreadMyStuff(this);
                }
                break;
            case GuestCurrentState.End:
                if (onceChk)
                {
                    onceChk = false;
                    ChangeMyActState(GuestActState.Walk);
                }
                break;
            case GuestCurrentState.Exit:
                if (onceChk)
                {
                    onceChk = false;
                    purpose = GuestController.Instance.GetJoinPoint();
                    ChangeMyActState(GuestActState.Walk);
                }
                break;
            case GuestCurrentState.Destroy:
                GuestController.Instance.OnDestroyGuest(gameObject);
                ObjectPool.Instance.Release<GuestCharacter>(gameObject);
                ResetGuest();
                break;
        }
    }

    private void Update()
    {
        ActStateProcess();
        CurrentStateProcess();
        if (purpose == null) return;

        if (isLookRot)
        {
            Quaternion targetRotation = Quaternion.LookRotation(purpose.position - transform.position);

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
                transform.position = Vector3.MoveTowards(transform.position, purpose.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = purpose.position;
                isMovePos = false;
                isPurposeRot = true;

                if (myCurrentState.Equals(GuestCurrentState.Exit))
                {
                    isPurposeRot = false;
                    onceChk = true;
                    myCurrentState++;
                }

                if (myCurrentState.Equals(GuestCurrentState.End))
                {
                    isPurposeRot = false;
                    onceChk = true;
                    myCurrentState++;
                }
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
                purpose = null;
                onceChk = true;
                if (myPurposeLineUp != null && myPurposeLineUp.parent.GetComponent<CashierGuest>() != null)
                    ChangeMyCurrentState(GuestCurrentState.BuyWait);
                else
                    myCurrentState++;
                ChangeMyActState(GuestActState.Idle);
            }
        }
    }

    public void ChangePurpose(Transform purpose)
    {
        if (purpose != null)
        {
            ResetMove();
            this.purpose = purpose;
            this.isLookRot = true;
        }
    }

    public void SelectStuff()
    {
        StuffObject[] stuffs = new[]
        {
            StuffObject.Burger, StuffObject.Donut, StuffObject.Snack, StuffObject.Coke, StuffObject.Juice, StuffObject.Yogurt
        }.OrderBy(x => Random.value).Take(Random.Range(1, 4)).ToArray();

        foreach (StuffObject s in stuffs)
        {
            hopeStuff.Add(s, Random.Range(0, 100) % 3 + 1);
            takeStuff.Add(s, 0);
        }

        myCurrentState++;
    }

    public void PickStuff()
    {
        putStuff = putStuffSlot.PutStuff(true);
        StuffObject so = putStuff.GetChild(0).GetComponent<Stuff>().stuff;
        putStuff.GetChild(0).gameObject.SetActive(false);
        putStuff.GetChild(0).SetParent(myBasket);
        hopeStuff[so]--;
        takeStuff[so]++;
        putStuff = null;
        onceChk = true;

        if (putStuffSlot.myStuff.Equals(StuffObject.None) || hopeStuff[so] == 0)
        {
            if (hopeStuff[so] == 0)
            {
                putStuffSlot = null;
            }
            ChangeMyCurrentState(GuestCurrentState.Select);
            ChangeMyActState(GuestActState.Idle);
        }
    }

    public void NextLineUp()
    {
        myPurposeLineUp = myPurposeLineUp.parent.GetChild(myPurposeLineUp.GetSiblingIndex() - 1);
        ChangeMyCurrentState(GuestCurrentState.Select);
        ChangePurpose(myPurposeLineUp);
    }

    public void ResetMove()
    {
        this.purpose = null;
        isMovePos = false;
        isPurposeRot = false;
        isLookRot = false;
    }

    public void ChkMyPurposeLineUp(Transform baseGuest)
    {
        if (myPurposeLineUp != null && myPurposeLineUp.parent == baseGuest)
        {
            ResetMove();
            ChangeMyCurrentState(GuestCurrentState.Select);
            ChangeMyActState(GuestActState.Idle);
        }
    }

    private void ResetGuest()
    {
        purposeObj = null;
        purpose = null;
        myPurposeLineUp = null;
        isLookRot = false;
        isMovePos = false;
        isPurposeRot = false;
        onceChk = false;
        putStuffSlot = null;
        putStuff = null;
        hopeStuff.Clear();
        takeStuff.Clear();
    }
}
