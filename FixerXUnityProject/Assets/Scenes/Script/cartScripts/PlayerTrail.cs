using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    GameObject trail1;
    GameObject trail2;
    
    void Start()
    {
        trail1 = transform.Find("Trail").transform.Find("Trail1").gameObject;
        trail2 = transform.Find("Trail").transform.Find("Trail2").gameObject;
    }

    public void DriftDraw()
    {
        trail1.GetComponent<TrailRenderer>().emitting = true;
        trail2.GetComponent<TrailRenderer>().emitting = true;
    }

    public void DriftRemove()
    {
        trail1.GetComponent<TrailRenderer>().emitting = false;
        trail2.GetComponent<TrailRenderer>().emitting = false;
    }
}
