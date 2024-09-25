using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Box : MonoBehaviour
{
    private Camera _cam;

    public Camera myCam { get => _cam; set => _cam = value; }

    public StuffObject stuff;
    [SerializeField] public Queue<Stuff> stuffs = new Queue<Stuff>();

    public Transform slots;

    private void Start()
    {
        // юс╫ц
        // OnCreateBox(stuff);
    }

    public void OnCreateBox(StuffObject stuff)
    {
        this.stuff = stuff;
        for (int i = 0; i < 20; i++)
        {
            GameObject s = Instantiate(Resources.Load($"Prefabs/Stuff/{stuff}") as GameObject);
            s.transform.SetParent(transform);
            s.GetComponent<Stuff>().stuff = stuff;
            s.SetActive(false);
            stuffs.Enqueue(s.GetComponent<Stuff>());
        }
    }

    public void OnDrop()
    {
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.SetParent(_cam.transform);
        transform.localPosition = new Vector3(0, -0.9f, 1.5f);
        transform.localRotation = Quaternion.Euler(-10f, 0, 0);
        transform.localScale = Vector3.one;
    }

    public void OnPut()
    {
        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.SetParent(null);
        Quaternion q = transform.localRotation;
        q.x = 0.0f; q.z = 0.0f;
        transform.localRotation = q;
        transform.localScale = Vector3.one;
        transform.GetComponent<Rigidbody>().AddForce(_cam.transform.forward * 20.0f, ForceMode.Impulse);
    }

    public void OnDisplay()
    {
        if (stuffs != null && stuffs.Count > 0)
        {
            if (slots.parent.GetComponent<Slots>().OnDisplayCheck(this.stuff))
            {
                slots.parent.GetComponent<Slots>().AddStuff(stuffs.Dequeue().transform);
            }
        }
    }
}
