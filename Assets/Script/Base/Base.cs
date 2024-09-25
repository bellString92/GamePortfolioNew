using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Base : MonoBehaviour
{
    private Material baseMaterial;
    private Material baseEmptyMaterial;
    private Transform[] myBaseArr;
    // Start is called before the first frame update
    void Start()
    {
        baseMaterial = Instantiate(Resources.Load<Material>("Material/Base"));
        baseEmptyMaterial = Instantiate(Resources.Load<Material>("Material/BaseEmpty"));
        myBaseArr = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform t in transform)
        {
            t.GetComponent<Renderer>().material = baseMaterial;
            t.GetComponent<Collider>().enabled = false;
            myBaseArr[i++] = t;
        }
    }

    public void OnChangeMyBaseEmpty(int index)
    {
        myBaseArr[index].GetComponent<Renderer>().material = baseEmptyMaterial;
        myBaseArr[index].GetComponent<Collider>().enabled = true;
    }

    public void OnChangeMyBase(int index)
    {
        myBaseArr[index].GetComponent<Renderer>().material = baseMaterial;
        myBaseArr[index].GetComponent<Collider>().enabled = false;
    }

    public void OnAllBaseView()
    {
        for (int i = 0; i < myBaseArr.Length; i++)
        {
            if (myBaseArr[i].childCount == 0)
            {
                myBaseArr[i].GetComponent<Renderer>().material = baseEmptyMaterial;
                myBaseArr[i].GetComponent<Collider>().enabled = true;
            }
        }
    }

    public void OffAllBaseView()
    {
        for (int i = 0; i < myBaseArr.Length; i++)
        {
            myBaseArr[i].GetComponent<Renderer>().material = baseMaterial;
            myBaseArr[i].GetComponent<Collider>().enabled = false;
        }
    }

}
