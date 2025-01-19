using UnityEngine;
using UnityEngine.UI;

public class HitDetector : MonoBehaviour
{
    [Header("판정 거리 설정")]
    public float perfectThreshold = 50.0f; // 퍼펙트 판정 거리
    public float goodThreshold = 100.0f;    // 굿 판정 거리
    public float badThreshold = 150.0f;     // 배드 판정 거리

    [Header("UI 연결")]
    public Image judgmentImage;           // 판정 이미지를 표시할 UI
    public Sprite perfectSprite;          // "Perfect" 이미지
    public Sprite goodSprite;             // "Good" 이미지
    public Sprite badSprite;              // "Bad" 이미지
    public Sprite missSprite;             // "Miss" 이미지

    [Header("이미지 표시 시간")]
    public float displayTime = 1.0f;      // 이미지 표시 시간
    private float timer = 0f;             // 이미지 숨기기 타이머

    [Header("점수제어")]
    public float perfectCombo = 16f;
    public float goodCombo = 8f;
    public float badCombo = 3f;
    public float missCombo = 0f;
    public PlayerController pcontroller;
    
    [Header("콤보")]
    public float combo = 0f;
    public bool comboBool = false;
    public Image comboImage; 
    public float fadeDuration = 1.0f; // 페이드 아웃 지속 시간 (초)
    public float fadeTimer = 1.0f; // 페이드 타이머

    //public float moveComboSpeed = PlayerController.moveSpeed;

    void Update()
    {
        //Debug.Log($"moveSpeed:{pcontroller.moveSpeed}");
        if (!GameManager.ManagerIns.isUseGMFunc){
            // 키 입력 처리
            if (Input.GetKeyDown(KeyCode.A)) { CheckNoteHit("A"); }
            if (Input.GetKeyDown(KeyCode.S)) { CheckNoteHit("S"); }
            if (Input.GetKeyDown(KeyCode.D)) { CheckNoteHit("D"); }
            if (Input.GetKeyDown(KeyCode.F)) { CheckNoteHit("F"); }

            // 이미지 표시 시간 관리
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0) { HideJudgmentImage(); }
            }

            //combo 이미지
            if(comboBool){
                if (fadeTimer < fadeDuration)
                {
                    fadeTimer += Time.deltaTime;

                    // 현재 타이머에 따라 알파 값 계산
                    float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);

                    // 이미지 색상 가져오기
                    Color color = comboImage.color;
                    color.a = alpha; // 알파 값 업데이트

                    // 색상 다시 적용
                    comboImage.color = color;

                    // 페이드가 완료되었는지 확인
                    if (fadeTimer >= fadeDuration)
                    {
                        Debug.Log("Fade in complete!");
                    }
                }
            }else{
                if (fadeTimer < fadeDuration)
                {
                    fadeTimer += Time.deltaTime;

                    // 현재 타이머에 따라 알파 값 계산
                    float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeDuration);

                    // 이미지 색상 가져오기
                    Color color = comboImage.color;
                    color.a = alpha; // 알파 값 업데이트

                    // 색상 다시 적용
                    comboImage.color = color;

                    // 페이드가 완료되었는지 확인
                    if (fadeTimer >= fadeDuration)
                    {
                        Debug.Log("Fade out complete!");
                    }
                }
            }
        }
    }

    void CheckNoteHit(string key)
    {
        // 히트존에 있는 노트 찾기
        Note note = FindNoteInHitzone(key);
        if (note != null)
        {
            float distance = Mathf.Abs(note.transform.position.x - transform.position.x);
            if (distance <= perfectThreshold)
            {
                combo++;
                pcontroller.moveSpeed = perfectCombo;
                ShowJudgmentImage(perfectSprite); // "Perfect" 이미지 표시
                Destroy(note.gameObject);          // 노트 제거
            }
            else if (distance <= goodThreshold)
            {
                pcontroller.moveSpeed = goodCombo;
                ShowJudgmentImage(goodSprite);    // "Good" 이미지 표시
                Destroy(note.gameObject);         // 노트 제거
            }
            else if (distance <= badThreshold)
            {
                pcontroller.moveSpeed = badCombo;
                ShowJudgmentImage(badSprite);     // "Bad" 이미지 표시
                Destroy(note.gameObject);         // 노트 제거
            }
            else
            {
                combo = 0f;
                pcontroller.moveSpeed = missCombo;
                //Debug.Log($"b");
                ShowJudgmentImage(missSprite);    // "Miss" 이미지 표시
               
            }
        }
        else
        {
            combo = 0f;
            pcontroller.moveSpeed = missCombo;
            ShowJudgmentImage(missSprite);        // "Miss" 이미지 표시
        }
        if(combo>=5){
            if(!comboBool){
                fadeTimer = 0f;
            }
            comboBool = true;
        }else{
            if(comboBool){
                fadeTimer = 0f;
            }
            comboBool = false;
        }
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

    Note FindNoteInHitzone(string key)
    {
        // 히트존에 있는 노트 중 해당 키와 일치하는 노트 찾기
        Note[] notes = FindObjectsOfType<Note>();
        foreach (Note note in notes)
        {
            //Debug.Log($"노트키: {note.key} , 키: {key} , 노트히트존: {note.isInHitzone}");
            if (note.isInHitzone && note.key == key)
            {
                return note;
            }
        }
        return null;
    }
}
