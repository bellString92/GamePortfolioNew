using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

enum TriggerDoor
{
    City, Store, Other
}

public class Player : AnimatorProperty
{
    public Camera myCam;
    public Camera myCam2;
    private Vector2 inputDir = Vector2.zero;
    private Vector3 moveDir;
    [SerializeField] private float moveSpeed = 5.0f;
    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    public GameObject actionKey;
    public bool doorTriggerChk = false;
    public bool onDoorTrigger = false;
    private TriggerDoor doorAct = TriggerDoor.Store;

    public GameObject playerCursor;

    public bool cursorChk = false;
    public bool slotChk = false;
    public bool onSlot = false;
    public bool onBox = false;
    public bool onTable = false;
    [SerializeField] private LayerMask actLayer;
    public Transform dropObject;

    private bool keyPressChk = true;
    private Coroutine keyPressCor = null;

    public Transform screenView;
    public bool screenViewChk = false;
    public bool onScreenView = false;
    public GameObject shopObj;

    public bool garbageChk = false;
    public bool onGarbage = false;

    public GameObject pricePopup;
    public bool priceChk = false;
    public bool onPrice = false;
    public int price = 0;
    public StuffObject stuff;
    public Slots slots;

    public bool openChk = false;
    public OpenStore openStore;

    public bool stuffChk = false;
    public Transform sellStuff = null;
    public int sellPrice = 0;
    public Cashier cashier;

    public bool sellChk = false;

    public bool bedChk = false;
    public Transform endDayObj = null;

    public Transform bedCam = null;
    public bool onBed { get; set; } = false;
    public Information infoObj;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        myCam = transform.GetComponentsInChildren<Camera>()[0];
        myCam2 = transform.GetComponentsInChildren<Camera>()[1];

        Clock.Instance.StartClock();
        //Global.Gold = 1000000;
    }

    // Update is called once per frame
    void Update()
    {
        if (myAnim.GetBool("OnStop"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (onScreenView)
                    ScreenViewExit(1.0f);
                else if (onPrice)
                {
                    OffPricePopup(price);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (onPrice)
                {
                    pricePopup.GetComponent<Price>().OnSave();
                }
            }
            return;
        }

        // 플레이어 이동
        inputDir.x = Input.GetAxisRaw("Horizontal");
        inputDir.y = Input.GetAxisRaw("Vertical");

        myAnim.SetFloat("x", inputDir.x);
        myAnim.SetFloat("y", inputDir.y);

        if (inputDir.x == 0 && inputDir.y == 0)
        {
            myAnim.SetBool("IsWalk", false);
        }
        else 
        {
            myAnim.SetBool("IsWalk", true);

            moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // 카메라 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -40f, 60f);

        myCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (doorTriggerChk)
        {
            actionKey.transform.GetChild(0).gameObject.SetActive(onDoorTrigger);
            doorTriggerChk = false;
        }

        Ray ray = myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 2.0f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 2.0f, actLayer))
        {
            if (hit.collider.tag.Equals("Bed"))
            {
                if (onBed) cursorChk = true;
            }
            else cursorChk = true;

            switch (hit.collider.tag)
            {
                case "Box":
                    if (!onBox)
                    {
                        dropObject = hit.transform.GetComponent<Box>().transform;
                        dropObject.GetComponent<Box>().myCam = myCam;
                    }
                    break;
                case "Slot":
                    priceChk = false;
                    if (onBox)
                    {
                        if (hit.collider.transform.parent.GetComponent<Table>().myBase != null)
                        {
                            slotChk = true;
                            dropObject.GetComponent<Box>().slots = hit.collider.transform.GetChild(0);
                        }
                    }
                    break;
                case "Table":
                    priceChk = false;
                    if (!onBox && !onTable)
                    {
                        dropObject = hit.transform.GetComponent<Table>().transform;
                        dropObject.GetComponent<Table>().myCam = myCam;
                    }
                    break;
                case "Base":
                    if (onTable)
                    {
                        dropObject.GetComponent<Table>().myBase = hit.transform;
                        dropObject.GetComponent<Table>().OnView();
                    }
                    break;
                case "Screen":
                    screenViewChk = true;
                    break;
                case "Garbage":
                    garbageChk = true;
                    break;
                case "Price":
                    priceChk = true;
                    slots = hit.collider.transform.parent.GetComponent<Slots>();
                    this.price = slots.price;
                    this.stuff = slots.myStuff;
                    pricePopup.GetComponent<Price>().player = this;
                    break;
                case "Open":
                    openChk = true;
                    break;
                case "Stuff":
                    sellStuff = hit.collider.transform;
                    stuffChk = true;
                    break;
                case "Sell":
                    sellChk = true;
                    break;
                case "Bed":
                    bedChk = true;
                    break;
            }
        }
        else
        {
            if (!onBox && !onTable)
                dropObject = null;
            cursorChk = false;

            if (onTable && !cursorChk && dropObject != null && dropObject.GetComponent<Table>() != null)
            {
                dropObject.GetComponent<Table>().OffView();
            }
            if (!onSlot) slotChk = false;
            if (!onScreenView) screenViewChk = false;
            if (!onGarbage) garbageChk = false;
            if (!onPrice) priceChk = false;
            if (!stuffChk) sellStuff = null;
            openChk = false;
            sellChk = false;
            bedChk = false;
        }

        playerCursor.transform.GetChild(0).gameObject.SetActive(!cursorChk);
        playerCursor.transform.GetChild(1).gameObject.SetActive(cursorChk);

        // E키
        if (Input.GetKey(KeyCode.E) && keyPressChk && keyPressCor == null)
        {
            keyPressChk = false;
            keyPressCor = StartCoroutine(WaitKeyPress(0.5f));
            if (onDoorTrigger)
            {
                if (doorAct.Equals(TriggerDoor.City))
                {
                    PlayerPrefs.SetInt("playerPosition", (int)PlayerPosition.CityNormal);
                }
                else if (doorAct.Equals(TriggerDoor.Store))
                {
                    PlayerPrefs.SetInt("playerPosition", (int)PlayerPosition.StoreNormal);
                }
                else if (doorAct.Equals(TriggerDoor.Other))
                {
                    PlayerPrefs.SetInt("playerPosition", (int)PlayerPosition.OtherNormal);
                }

                SceneManager.LoadScene((int)doorAct);
                onDoorTrigger = false;
            }
            else if (priceChk)
            {
                onPrice = true;
                myAnim.SetBool("IsWalk", false);
                myAnim.SetBool("OnStop", true);
                Cursor.lockState = CursorLockMode.None;
                pricePopup.GetComponent<Price>().OnOpenPricePopup(this.price, this.stuff);
            }
            else if (openChk)
            {
                openStore.OnOpenToggle();
            }
            else if (screenViewChk)
            {
                ScreenViewMove(1.0f);
                playerCursor.transform.GetChild(0).gameObject.SetActive(false);
                playerCursor.transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (dropObject != null)
            {
                if (dropObject.GetComponent<Box>() != null)
                {
                    if (!onBox)
                    {
                        if (cursorChk)
                        {
                            dropObject.GetComponent<Box>().OnDrop();
                            dropObject.GetComponent<Collider>().enabled = false;
                            onBox = true;
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Table"))
                            {
                                obj.GetComponent<Collider>().enabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (cursorChk)
                        {
                            if (garbageChk)
                            {
                                dropObject.GetComponent<Box>().OnDestroyBox();
                            }
                            else if (slotChk)
                            {
                                onSlot = true;
                                dropObject.GetComponent<Box>().OnDisplay();
                            }
                        }

                        if (!cursorChk || cursorChk && (garbageChk || !slotChk))
                        {
                            if (!garbageChk) dropObject.GetComponent<Box>().OnPut();
                            dropObject.GetComponent<Collider>().enabled = true;
                            onBox = false;
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Table"))
                            {
                                obj.GetComponent<Collider>().enabled = true;
                            }
                        }
                    }
                }

                if (dropObject.GetComponent<Table>() != null)
                {
                    if (!onTable)
                    {
                        if (cursorChk)
                        {
                            dropObject.GetComponent<Table>().OnDrop();
                            onTable = true;
                        }
                    }
                    else
                    {
                        if (cursorChk)
                        {
                            if (dropObject.GetComponent<Table>().OnDisplay())
                                onTable = false;
                        }
                    }
                }
            }
            else if (stuffChk && sellStuff != null)
            {
                cashier.AddStuff(sellStuff.GetComponent<Stuff>());
            }
            else if (sellChk)
            {
                cashier.SellStuffs();
            }
            else if (bedChk && onBed)
            {
                myAnim.SetBool("OnStop", true);
                myCam.transform.SetParent(bedCam);
                myCam.transform.localPosition = Vector3.zero;
                myCam.transform.localRotation = Quaternion.identity;

                myAnim.applyRootMotion = true;
                transform.rotation = Quaternion.LookRotation(-transform.forward);

                playerCursor.transform.GetChild(0).gameObject.SetActive(false);
                playerCursor.transform.GetChild(1).gameObject.SetActive(false);

                Clock.Instance.StopClock();

                myAnim.SetBool("IsSit", true);
                myAnim.SetTrigger("OnSit");

            }
        }
    }

    IEnumerator WaitKeyPress(float delay)
    {
        yield return new WaitForSeconds(delay);
        keyPressChk = true;
        keyPressCor = null;
    }

    void ScreenViewMove(float time)
    {
        myAnim.SetBool("OnStop", true);
        myAnim.SetBool("IsWalk", false);
        myCam2.gameObject.SetActive(true);
        myCam.gameObject.SetActive(false);
        LeanTween.rotate(myCam2.gameObject, screenView.rotation.eulerAngles, time).setOnComplete(ShowScreenPopup);
        LeanTween.move(myCam2.gameObject, screenView, time);
    }

    void ShowScreenPopup()
    {
        onScreenView = true;
        shopObj.SetActive(true);
        LeanTween.scale(shopObj, new Vector3(1, 1, 1), 0.3f).setEase(LeanTweenType.once);
        Cursor.lockState = CursorLockMode.None;
    }

    void ScreenViewExit(float time)
    {
        ExitScreenPopup(time);
    }

    void ExitScreenPopup(float time)
    {
        onScreenView = false;
        Cursor.lockState = CursorLockMode.Locked;
        LeanTween.scale(shopObj, new Vector3(0.01f, 0.01f, 0.01f), 0.3f).setEase(LeanTweenType.once).setOnComplete(() => {
            shopObj.SetActive(false);
            PlayerViewMove(time);
        });
    }

    void PlayerViewMove(float time)
    {
        LeanTween.move(myCam2.gameObject, myCam.transform, time);
        LeanTween.rotate(myCam2.gameObject, myCam.transform.rotation.eulerAngles, time).setOnComplete(() =>
        {
            myAnim.SetBool("OnStop", false);
            myAnim.SetBool("IsWalk", true);
            myCam2.gameObject.SetActive(false);
            myCam.gameObject.SetActive(true);
        });
    }

    public void OffPricePopup(int price)
    {
        this.price = price;
        myAnim.SetBool("OnStop", false);
        onPrice = false;
        Cursor.lockState = CursorLockMode.Locked;
        pricePopup.SetActive(false);
        slots.OnChangePrice(price);
    }

    public void EndDay()
    {
        StartCoroutine(EndDayCor());
    }

    IEnumerator EndDayCor()
    {
        float alpha = 0.0f;
        endDayObj.GetChild(0).gameObject.SetActive(true);
        
        Color color = endDayObj.GetChild(0).GetComponent<Image>().color;

        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime * 0.5f;
            color.a = alpha;
            endDayObj.GetChild(0).GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1.0f;
        endDayObj.GetChild(0).GetComponent<Image>().color = color;

        ShowInformation();
    }

    void ShowInformation()
    {
        infoObj.player = this;
        Cursor.lockState = CursorLockMode.None;
        infoObj.StartInformation();
    }

    public void ResetDay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Global.ResetDay(this);
    }

    public void HideInformation()
    {
        StartCoroutine(StartDayCor());
    }

    IEnumerator StartDayCor()
    {
        float alpha = 1.0f;

        Color color = endDayObj.GetChild(0).GetComponent<Image>().color;

        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime * 0.5f;
            color.a = alpha;
            endDayObj.GetChild(0).GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 0.0f;
        endDayObj.GetChild(0).GetComponent<Image>().color = color;
        endDayObj.GetChild(0).gameObject.SetActive(false);

        StartDay();
    }

    void StartDay()
    {
        myAnim.SetBool("IsSit", false);
    }

    public void MovePlayer()
    {
        playerCursor.transform.GetChild(0).gameObject.SetActive(true);
        playerCursor.transform.GetChild(1).gameObject.SetActive(false);

        myCam.transform.SetParent(transform);
        myAnim.applyRootMotion = false;
        myCam.transform.localPosition = myCam2.transform.localPosition;
        myCam.transform.localRotation = myCam2.transform.rotation;
        myAnim.SetBool("OnStop", false);

        Clock.Instance.StartClock();
    }
}
