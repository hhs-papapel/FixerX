using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static float time = 0;
    string timeStr;
    public Text timer;

    void Update()
    {
        if (!GameManager.ManagerIns.isUseGMFunc) time += Time.deltaTime;
        timeStr = "" + time.ToString("00.00");
        timeStr = timeStr.Replace(".", ":");
        timer.text = timeStr;
    }
}
