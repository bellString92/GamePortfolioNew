using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestPos : MonoBehaviour
{
    public Transform purposeObj;
    private void Start()
    {
        GuestController.Instance.joinPoint = transform;
        GuestController.Instance.purposeObj = purposeObj;
    }
}
