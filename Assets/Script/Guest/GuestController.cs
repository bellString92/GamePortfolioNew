using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public enum GuestCharacter
{
    NonPBR, NonPBR2//, Kachujin 
}

public class GuestController : Singleton<GuestController>
{
    public int count = 0;
    public int maxCount = 10;
    public bool isFull = false;
    public Coroutine waitCor = null;
    public GameObject[] guestArr = null;
    public Transform joinPoint;
    public Transform purposeObj;
    public bool isOpen = false;

    private int joinNum = 0;

    private void Awake()
    {
        base.Initialize();
    }

    private void Start()
    {
        guestArr = new GameObject[maxCount];
        for (int i = 0; i < guestArr.Length; i++)
        {
            GuestCharacter guest = (GuestCharacter)Random.Range(0, System.Enum.GetValues(typeof(GuestCharacter)).Length);
            guestArr[i] = ObjectPool.Instance.Instantiate<GuestCharacter>(Resources.Load($"Prefabs/Guest/{guest}") as GameObject, ObjectPool.Instance.transform);
            guestArr[i].name = guest.ToString();
            guestArr[i].SetActive(false);
            
        }
        for (int i = 0; i < guestArr.Length; i++)
        {
            ObjectPool.Instance.Release<GuestCharacter>(guestArr[i]);
            guestArr[i] = null;
        }
    }

    public void OnCreateGuest()
    {
        isOpen = true;
        waitCor = StartCoroutine(CreateWait());
    }

    public void CreateGuest()
    {
        if (!isFull && isOpen)
        {
            bool chk = false;
            for (int i = 0; i < guestArr.Length; i++)
            {
                if (guestArr[i] == null)
                {
                    GuestCharacter guest = (GuestCharacter)Random.Range(0, System.Enum.GetValues(typeof(GuestCharacter)).Length);
                    guestArr[i] = ObjectPool.Instance.Instantiate<GuestCharacter>(Resources.Load($"Prefabs/Guest/{guest}") as GameObject, joinPoint);
                    Guest guestComp = guestArr[i].GetComponent<Guest>();
                    guestArr[i].name = guest.ToString();
                    guestComp.purposeObj = purposeObj;
                    guestComp.purpose = purposeObj.GetComponent<PurposePos>().doorObj;
                    guestComp.ChangeMyActState(GuestActState.Walk);
                    guestComp.ChangeMyCurrentState(GuestCurrentState.Start);
                    count++;
                    Global.statistics.guest++;
                    chk = true;
                    break;
                }
            }
            if (count >= 10) isFull = true;
            if (!isFull && chk) waitCor = StartCoroutine(CreateWait());
        }
    }


    private IEnumerator CreateWait()
    {
        int rndNum = Random.Range(0, 1000) % 5 + 1;
        yield return new WaitForSeconds(rndNum);
        waitCor = null;
        CreateGuest();
    }

    public void StopGuest()
    {
        isOpen = false;
        if (waitCor != null)
        {
            StopCoroutine(waitCor);
            waitCor = null;
        }
    }

    public Transform UpdateGuestPurpose(GuestCurrentState gcs, Guest guest)
    {
        PurposePos pp = purposeObj.GetComponent<PurposePos>();
        switch (gcs)
        {
            case GuestCurrentState.Door:
                if (joinNum >= 10) joinNum = 0;
                return pp.joinObj.GetChild(joinNum++);
            case GuestCurrentState.Select:
                bool chk = false;
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Slot"))
                {
                    if (guest.hopeStuff.ContainsKey(obj.GetComponent<Slots>().myStuff) && guest.hopeStuff[obj.GetComponent<Slots>().myStuff] > 0)
                    {
                        chk = true;
                        Transform baseGuestObj = pp.baseObj.GetChild(obj.GetComponent<Slots>().transform.parent.parent.GetSiblingIndex());
                        BaseGuest baseGuest = baseGuestObj.GetComponent<BaseGuest>();
                        if (guest.myPurposeLineUp != null && guest.myPurposeLineUp.parent == baseGuestObj)
                        {
                            if (guest.myPurposeLineUp.GetSiblingIndex() == 0)
                                guest.ChangeMyCurrentState(GuestCurrentState.Pick);
                            else
                                guest.ChangeMyCurrentState(GuestCurrentState.PickWait);
                            return null;
                        }
                        else if (!baseGuest.IsFull())
                        {
                            if (guest.myPurposeLineUp != null && guest.myPurposeLineUp.parent != baseGuestObj)
                            {
                                BaseGuest bg = guest.myPurposeLineUp.parent.GetComponent<BaseGuest>();
                                bg.DequeueGuest();
                                Queue<Guest> guestLineUps = bg.GetAllGuest();
                                if (guestLineUps != null)
                                {
                                    while (guestLineUps.Count > 0)
                                    {
                                        guestLineUps.Dequeue().NextLineUp();
                                    }
                                }
                            }
                            int cp = baseGuest.GetCurPoint();
                            baseGuest.EnqueueGuest(guest);
                            guest.myPurposeLineUp = baseGuestObj.GetChild(cp);
                            return guest.myPurposeLineUp;
                        }
                        break;
                    }
                }

                if (!chk)
                {
                    foreach (StuffObject key in guest.hopeStuff.Keys.ToArray())
                    {
                        guest.hopeStuff[key] = 0;
                    }

                    int takeNum = 0;
                    foreach (KeyValuePair<StuffObject, int> take in guest.takeStuff)
                    {
                        if (take.Value > 0)
                        {
                            takeNum++;
                            if (guest.myPurposeLineUp != null)
                            {
                                BaseGuest bg = guest.myPurposeLineUp.parent.GetComponent<BaseGuest>();
                                bg.DequeueGuest();
                                Queue<Guest> guestLineUps = bg.GetAllGuest();
                                if (guestLineUps != null)
                                {
                                    while (guestLineUps.Count > 0)
                                    {
                                        guestLineUps.Dequeue().NextLineUp();
                                    }
                                }
                            }

                            CashierGuest cashierGuest = pp.buyObj.GetComponent<CashierGuest>();
                            if (!cashierGuest.IsFull())
                            {
                                int cp = cashierGuest.GetCurPoint();
                                cashierGuest.EnqueueGuest(guest);
                                guest.myPurposeLineUp = pp.buyObj.GetChild(cp);
                                return pp.buyObj.GetChild(cp);
                            }
                        }
                    }

                    if (takeNum == 0 && guest.myPurposeLineUp != null)
                    {
                        guest.myPurposeLineUp.parent.GetComponent<BaseGuest>().DequeueGuest();
                        guest.myPurposeLineUp = null;
                    }

                    guest.myCurrentState = GuestCurrentState.End;
                    return pp.doorObj;
                }
                break;
            case GuestCurrentState.Buy:
                if (pp.buyObj.GetComponent<CashierGuest>().takeStuffNum == 0)
                {
                    guest.ChangeMyCurrentState(GuestCurrentState.Pay);
                }
                break;
            case GuestCurrentState.Complete:
                {
                    pp.buyObj.GetComponent<CashierGuest>().DequeueGuest();

                    Queue<Guest> guestLineUps = pp.buyObj.GetComponent<CashierGuest>().GetAllGuest();
                    if (guestLineUps != null)
                    {
                        while (guestLineUps.Count > 0)
                        {
                            guestLineUps.Dequeue().NextLineUp();
                        }
                    }

                    guest.myPurposeLineUp = null;
                    Global.statistics.buyGuest++;
                    guest.ChangeMyCurrentState(GuestCurrentState.End);
                    return pp.doorObj;
                }
        }

        return null;
    }

    public void OnDestroyGuest(GameObject guest)
    {
        for (int i = 0; i < guestArr.Length; i++)
        {
            if (guestArr[i] == guest)
            {
                guestArr[i] = null;
                count--;
                isFull = false;
                //CreateGuest(); // 왜 Destroy인데 게스트를 생성했지???? 
                if (waitCor == null) waitCor = StartCoroutine(CreateWait());
            }
        }
    }

    public Transform GetJoinPoint()
    {
        return joinPoint;
    }

    public void EmptyBaseStuff(BaseGuest bg)
    {
        Queue<Guest> guests = bg.GetAllGuest();
        while (bg.fillCount > 0)
        {
            bg.DequeueGuest();
        }

        while (guests.Count > 0)
        {
            Guest guest = guests.Dequeue();
            guest.ResetMove();
            guest.myPurposeLineUp = null;
            guest.ChangeMyCurrentState(GuestCurrentState.Select);
            guest.ChangeMyActState(GuestActState.Idle);
        }

        for (int i = 0; i < guestArr.Length; i++)
        {
            if (guestArr[i] != null && guestArr[i].GetComponent<Guest>() != null && guestArr[i].GetComponent<Guest>().gameObject.activeInHierarchy)
                guestArr[i].GetComponent<Guest>().ChkMyPurposeLineUp(bg.transform);
        }
    }

    public void SpreadMyStuff(Guest guest)
    {
        Transform sell = purposeObj.GetComponent<PurposePos>().sellObj;
        StartCoroutine(SpreadMyStuffCor(guest.myBasket, sell));
    }

    IEnumerator SpreadMyStuffCor(Transform basket, Transform sell)
    {
        int i = 0;
        foreach (Stuff t in basket.GetComponentsInChildren<Stuff>(true))
        {
            t.transform.SetParent(sell.GetChild(i++));
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;
            t.gameObject.SetActive(true);
            t.GetComponent<Collider>().enabled = true;
            yield return null;
        }
    }
}
