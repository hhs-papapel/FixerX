using UnityEngine;
using UnityEngine.UI;

public class NoteMovement : MonoBehaviour
{
    public Transform hitZone;       // 중앙 히트존 위치 (목표 위치)
    public float speed = 3f;        // 이동 속도
    public float delayAtTarget = 0.01f; // 목표 지점에서 머무르는 유예 시간

    private Vector3 targetPosition; // 목표 위치
    private bool isAtTarget = false; // 목표 지점에 도달했는지 여부
    private float stayTimer = 0f;   // 유예 시간 타이머

    [Header("이미지 표시 시간")]
    public float displayTime = 1.0f;      // 이미지 표시 시간
    private float timer = 0f;             // 이미지 숨기기 타이머
    
    [Header("점수제어")]
    public float perfectCombo = 16f;
    public float missCombo = 0f;
    public PlayerController pcontroller;

    public Image judgmentImage;           // 판정 이미지를 표시할 UI
    public Sprite missSprite;             // "Miss" 이미지
    
    public HitDetector hitDetector;


    void Start()
    {
        // 히트존 위치를 목표 위치로 설정
        if (hitZone != null)
        {
            targetPosition = hitZone.position;
        }
        else
        {
            Debug.LogError("HitZone 위치가 설정되지 않았습니다!");
        }
    }

    void Update()
    {
        if (!GameManager.ManagerIns.isUseGMFunc){
            if (!isAtTarget)
            {
                // 노트를 중앙으로 이동
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // 목표 지점에 도달했는지 확인
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    isAtTarget = true;
                    stayTimer = delayAtTarget;
                    transform.position = targetPosition;
                }
            }
            else
            {
                // 목표 지점에서 유예 시간 카운트 다운
                stayTimer -= Time.deltaTime;

                // 유예 시간이 끝나면 노트를 제거
                if (stayTimer <= 0)
                {
                    Destroy(gameObject);

                if(hitDetector.comboBool){
                    hitDetector.fadeTimer = 0f;
                }
                hitDetector.comboBool = false;

                    pcontroller.moveSpeed = missCombo;
                    ShowJudgmentImage(missSprite);  
                }
            }
        }
    }

    public void DestroyNote()
    {
        // 외부 입력에 의해 노트 제거
        Destroy(gameObject);
    }

        void ShowJudgmentImage(Sprite sprite)
    {
        if (judgmentImage != null)
        {
             judgmentImage.sprite = sprite;  // 스프라이트 변경
            judgmentImage.enabled = true;   // 이미지 활성화
            timer = displayTime;            // 타이머 초기화
            //Debug.Log($"이미지 활성화: {sprite.name}"); // 디버그 메시지
        }
        else
        {
            //Debug.LogError("Judgment Image가 설정되지 않았습니다!");
        }
    }

    void HideJudgmentImage()
    {
        if (judgmentImage != null)
        {
            judgmentImage.enabled = false; // 이미지를 숨김
            //Debug.Log("이미지가 숨겨졌습니다."); // 디버그 메시지
            
                pcontroller.moveSpeed = perfectCombo;
        }
    }
}
