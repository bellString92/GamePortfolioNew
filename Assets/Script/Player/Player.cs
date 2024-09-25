using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public bool onDoorTriggerChk = false;
    public bool onDoorTrigger = false;
    private TriggerDoor doorAct = TriggerDoor.Store;

    public GameObject playerCursor;

    public bool onCursorTriggerChk = false;
    public bool onBoxTrigger = false;
    public bool onTableTrigger = false;
    [SerializeField] private LayerMask actLayer;
    public Transform dropObject;

    private bool OnKeyPressChk = true;
    private Coroutine keyPressCor = null;

    public Transform screenView;
    public bool screenViewChk = false;
    public GameObject shopObj;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        myCam = transform.GetComponentsInChildren<Camera>()[0];
        myCam2 = transform.GetComponentsInChildren<Camera>()[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (myAnim.GetBool("OnStop"))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenViewExit(1.0f);
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
        xRotation = Mathf.Clamp(xRotation, -40f, 40f);

        myCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (onDoorTriggerChk)
        {
            actionKey.transform.GetChild(0).gameObject.SetActive(onDoorTrigger);
            onDoorTriggerChk = false;
        }

        Ray ray = myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 2.0f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 2.0f, actLayer))
        {
            onCursorTriggerChk = true;
            switch (hit.collider.tag)
            {
                case "Box":
                    dropObject = hit.transform.GetComponent<Box>().transform;
                    dropObject.GetComponent<Box>().myCam = myCam;
                    break;
                case "Slot":
                    if (onBoxTrigger)
                        dropObject.GetComponent<Box>().slots = hit.collider.transform.GetChild(0);
                    break;
                case "Table":
                    if (!onBoxTrigger && !onTableTrigger)
                    {
                        dropObject = hit.transform.GetComponent<Table>().transform;
                        dropObject.GetComponent<Table>().myCam = myCam;
                    }
                    break;
                case "Base":
                    if (onTableTrigger)
                    {
                        dropObject.GetComponent<Table>().myBase = hit.transform;
                        dropObject.GetComponent<Table>().OnView();
                    }
                    break;
                case "Screen":
                    screenViewChk = true;
                    break;
            }
        }
        else
        {
            if (!onBoxTrigger && !onTableTrigger)
                dropObject = null;
            onCursorTriggerChk = false;

            if (onTableTrigger && !onCursorTriggerChk && dropObject != null && dropObject.GetComponent<Table>() != null)
            {
                dropObject.GetComponent<Table>().OffView();
            }
        }

        playerCursor.transform.GetChild(0).gameObject.SetActive(!onCursorTriggerChk);
        playerCursor.transform.GetChild(1).gameObject.SetActive(onCursorTriggerChk);

        // E키
        if (Input.GetKey(KeyCode.E) && OnKeyPressChk && keyPressCor == null)
        {
            OnKeyPressChk = false;
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
            else if (dropObject != null)
            {
                if (dropObject.GetComponent<Box>() != null)
                {
                    if (!onBoxTrigger)
                    {
                        if (onCursorTriggerChk)
                        {
                            dropObject.GetComponent<Box>().OnDrop();
                            onBoxTrigger = true;
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Table"))
                            {
                                obj.GetComponent<Collider>().enabled = false;
                            }
                        }
                    }
                    else if (onBoxTrigger)
                    {
                        if (onCursorTriggerChk)
                        {
                            dropObject.GetComponent<Box>().OnDisplay();
                        }
                        else
                        {
                            dropObject.GetComponent<Box>().OnPut();
                            onBoxTrigger = false;
                            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Table"))
                            {
                                obj.GetComponent<Collider>().enabled = true;
                            }
                        }
                    }
                }

                if (dropObject.GetComponent<Table>() != null)
                {
                    if (!onTableTrigger)
                    {
                        if (onCursorTriggerChk)
                        {
                            dropObject.GetComponent<Table>().OnDrop();
                            onTableTrigger = true;
                        }
                    }
                    else if (onTableTrigger)
                    {
                        if (onCursorTriggerChk)
                        {
                            if (dropObject.GetComponent<Table>().OnDisplay())
                                onTableTrigger = false;

                        }
                    }
                }
            }

            if (screenViewChk)
            {
                ScreenViewMove(1.0f);
                playerCursor.transform.GetChild(0).gameObject.SetActive(false);
                playerCursor.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

    }

    IEnumerator WaitKeyPress(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnKeyPressChk = true;
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
        dropObject = null;
        LeanTween.scale(shopObj, new Vector3(0.01f, 0.01f, 0.01f), 0.3f).setEase(LeanTweenType.once).setOnComplete(() => {
            shopObj.SetActive(false);
            PlayerViewMove(time);
            Cursor.lockState = CursorLockMode.Locked;
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

}
