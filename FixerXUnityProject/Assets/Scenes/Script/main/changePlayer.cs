using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changePlayer : MonoBehaviour
{
    public GameObject nextCar;
    public float nextnum = 1;
    void OnMouseDown()
    {
        // 오브젝트가 클릭되었을 때 실행되는 코드
        gameObject.SetActive(false);
        nextCar.SetActive(true);
        GlobalUser.carNumber = nextnum;
    }
}
