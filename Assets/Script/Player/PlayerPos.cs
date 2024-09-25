using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum PlayerPosition
{
    Start, CityNormal, StoreNormal, OtherNormal
}

public class PlayerPos : MonoBehaviour
{

    private Vector3 playerPos = Vector3.zero;
    private Quaternion playerRot = Quaternion.identity;
    private GameObject player;
    private PlayerPosition playerPosition = PlayerPosition.Start;

    [SerializeField] private GameObject actionKey;
    [SerializeField] private GameObject playerCursor;

    public Transform screenView;
    public GameObject shopObj;
    private void Awake()
    {
        PlayerPrefs.SetInt("playerPosition", (int)PlayerPosition.StoreNormal);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = (PlayerPosition)PlayerPrefs.GetInt("playerPosition");
        if (playerPosition.Equals(PlayerPosition.Start))
        {
            playerPos = new Vector3(27.32172f, 6f, -176.27f);
            playerRot = Quaternion.identity;
        }
        else if (playerPosition.Equals(PlayerPosition.CityNormal))
        {
            playerPos = new Vector3(38.2783432f, 5.43676472f, -166.741638f);
            playerRot = new Quaternion(0, 180f, 0, 1);

        }
        else if (playerPosition.Equals(PlayerPosition.StoreNormal))
        {
            playerPos = Vector3.zero;
            playerRot = Quaternion.identity;
        }

        shopObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        shopObj.SetActive(false);

        player = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        player.transform.SetParent(null);
        player.name = "Player";
        player.transform.localPosition = playerPos;
        player.transform.localRotation = playerRot;
        player.GetComponent<Player>().actionKey = actionKey;
        player.GetComponent<Player>().playerCursor = playerCursor;
        player.GetComponent<Player>().screenView = screenView;
        player.GetComponent<Player>().shopObj = shopObj;
        
    }
}
