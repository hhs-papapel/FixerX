using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintVelocity : MonoBehaviour
{
    int velocity;
    string velocityStr;
    public Text veloTxt;
    Vector3 playerVelocity;

    void Update()
    {
        playerVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().velocity;
        velocity = (int)playerVelocity.magnitude;
        velocityStr = velocity.ToString();
        veloTxt.text = velocityStr;
       // Debug.Log($"velocityStr: {veloTxt.text}"); 
    }
}
