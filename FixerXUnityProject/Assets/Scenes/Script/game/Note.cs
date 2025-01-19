using UnityEngine;

public class Note : MonoBehaviour
{
    public string key;           // 노트와 연결된 키 (예: "A", "S", "D", "F")
    public bool isInHitzone = false; // 노트가 히트존에 있는지 여부
    public float hitTime;        // 노트의 예상 히트 시간
 // 트리거에 들어왔을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject.name} entered the trigger of {gameObject.name}");
        isInHitzone = true;
        // 특정 태그를 가진 객체에 대해 작동
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone!");
        }
    }

}
