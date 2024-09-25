using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TableObject
{
    None, Shelf, Refrigerator
}

public class Table : MonoBehaviour
{
    private Camera _cam;
    public Transform _base;
    public TableObject tableObject = TableObject.None;
    public MenuDesc myMenuDesc;

    public Camera myCam { get => _cam; set => _cam = value; }
    public Transform myBase { get => _base; set => _base = value; }


    private void Start()
    {
        if (_base == null && transform.parent != null) _base = transform.parent;
    }

    public void OnDrop()
    {
        transform.SetParent(_cam.transform);
        gameObject.SetActive(false);
        if (_base != null)
        {
            _base.parent.GetComponent<Base>().OnChangeMyBaseEmpty(_base.GetSiblingIndex());
            _base.parent.GetComponent<Base>().OnAllBaseView();
        }
    }

    public void OnView()
    {
        transform.SetParent(_base);
        transform.localScale = new Vector3(0.65f, 100, 0.5f);
        transform.localPosition = new Vector3(-0.5f, 80, 0);
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }

    public void OffView()
    {
        _base = null;
        transform.SetParent(_cam.transform);
        gameObject.SetActive(false);
    }

    public bool OnDisplay()
    {
        if (_base == null) return false;
        transform.SetParent(_base);
        transform.localScale = new Vector3(0.65f, 100, 0.5f);
        transform.localPosition = new Vector3(-0.5f, 80, 0);
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
        if (_base != null)
        {
            _base.parent.GetComponent<Base>().OnChangeMyBase(_base.GetSiblingIndex());
            _base.parent.GetComponent<Base>().OffAllBaseView();
        }
        return true;
    }
}
