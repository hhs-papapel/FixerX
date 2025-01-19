using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingPlayer : MonoBehaviour
{
    public float bounceHeight = 1f;   // 튀어 오를 높이 (월드 좌표에서 1 단위)
    public float bounceSpeed = 1f;   // 튕기는 속도
    public float timeOffset = 0f;    // 다른 요소와 엇갈리게 하기 위한 시간 오프셋

    private float startY;            // 원래 위치 Y (월드 좌표 기준)

    void Start()
    {
        // 오브젝트의 월드 좌표 기준 시작 위치 저장
        startY = transform.position.y;
    }

    void Update()
    {
        // Y축으로 위아래로 움직이도록 처리, timeOffset을 추가하여 시간 차이를 두고 움직임
        float bounce = Mathf.Sin((Time.time + timeOffset) * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(transform.position.x, startY + bounce, transform.position.z);
    }
}