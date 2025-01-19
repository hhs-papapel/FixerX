using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{
    Animator ani;
    Rigidbody rig;
    GameObject menu;
    PlayerTrail playerTrail;
    PlayerSound playerSound;
    
    Vector3 firstPos;
    public float moveSpeed = 8f;
    public float slowSpeed = 100f;
    public float upDrag = 1.5f;
    float dragSaved;
    public bool isShift = false;
    bool isUp = false;
    bool isDown = false;
    bool isLeft = false;
    bool isRight = false;
    public enum PlayerState
    {
        Drive,
        Idle
    }
    public PlayerState playerState = PlayerState.Idle;


    void Start()
    { 
        ani = GetComponent<Animator>(); 
        rig = GetComponent<Rigidbody>(); 
        playerTrail = GetComponent<PlayerTrail>();
        playerSound = GetComponent<PlayerSound>();
        menu = GameObject.Find("Canvas").transform.Find("Menu").gameObject;
        menu.SetActive(false); // 리플레이 할 때를 대비
        dragSaved = rig.drag; // 현재 drag 저장
        firstPos = transform.position; // 시작할 때 위치 저장(나중에 떨어지면 시작 지점으로 돌아옴)
    }



    void Update()
    {
        // 게임 도중 메뉴창 열기
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            GameManager.ManagerIns.ManageMenu();
        }

        // 카운트 다운이나 메뉴창 띄었을 때 이동 입력 받지x
        if (GameManager.ManagerIns.isUseGMFunc) 
            return;
        
        // ==== 이동 키 입력 ====
        if (Input.GetKey(KeyCode.UpArrow))
        {
            isUp = true;
            if (Input.GetKey(KeyCode.LeftShift))
            {   
                isShift = true;
                rig.drag = upDrag;
                BoosterManager.dirIns.IncreaseBoosterGage();
                playerTrail.DriftDraw();
                rig.AddForce(transform.forward * (moveSpeed * 100 - (slowSpeed*100)) * Time.deltaTime);
            }
            else
            {
                rig.AddForce(transform.forward * moveSpeed * 100 * Time.deltaTime);
            }
            
            playerState = PlayerState.Drive;

            if (isLeft) ani.Play("Ani_Left");
            else if (isRight) ani.Play("Ani_Right");
            else ani.Play("Ani_Forward");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isDown = true;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isShift = true;
                rig.drag = upDrag;
                BoosterManager.dirIns.IncreaseBoosterGage();
                playerTrail.DriftDraw();
                rig.AddForce(-transform.forward * (moveSpeed * 100 - (slowSpeed * 100)) * Time.deltaTime);
            }
            else
            {
                rig.AddForce(-transform.forward * moveSpeed * 100 * Time.deltaTime);
            }

            playerState = PlayerState.Drive;

            if (isLeft) ani.Play("Ani_Left");
            else if (isRight) ani.Play("Ani_Right");
            else ani.Play("Ani_Forward");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isLeft = true;
            if (isUp || isDown)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    isShift = true;
                    rig.drag = upDrag;
                    BoosterManager.dirIns.IncreaseBoosterGage();
                    playerTrail.DriftDraw();
                    rig.AddForce(-transform.right * (moveSpeed * 100 - (slowSpeed * 100)) * Time.deltaTime);
                    transform.Rotate(0, -1, 0);
                    playerState = PlayerState.Drive;
                }
                else
                {
                    rig.AddForce(-transform.right * moveSpeed * 100 * Time.deltaTime);
                    transform.Rotate(0, -0.5f, 0);
                    playerState = PlayerState.Drive;
                }
            }
            ani.Play("Ani_Left");   
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isRight = true;
            if (isUp || isDown){
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    isShift = true;
                    BoosterManager.dirIns.IncreaseBoosterGage();
                    rig.drag = upDrag;
                    playerTrail.DriftDraw();
                    rig.AddForce(transform.right * (moveSpeed * 100 - (slowSpeed * 100)) * Time.deltaTime);
                    transform.Rotate(0, 1, 0);
                    playerState = PlayerState.Drive;
                }
                else
                {
                    rig.AddForce(transform.right * moveSpeed * 100 * Time.deltaTime);
                    transform.Rotate(0, 0.5f, 0);
                    playerState = PlayerState.Drive;
                }
            }
            ani.Play("Ani_Right");
        }
       
        if (Input.GetKeyUp(KeyCode.UpArrow)) isUp = false;
        if (Input.GetKeyUp(KeyCode.DownArrow)) isDown = false;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) isLeft = false;
        if (Input.GetKeyUp(KeyCode.RightArrow)) isRight = false;
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isShift = false;
            rig.drag = dragSaved; // 다시 원래 drag로
            playerTrail.DriftRemove();
        }
        // 속력이 줄면 Idle애니메이션으로 전환
        if (rig.velocity.z < 0.1 && rig.velocity.z > -0.1 && !isLeft && !isRight)
        {
            playerState = PlayerState.Idle;
            ani.Play("Ani_Idle");
        }
        playerSound.Playsound();
        if (Input.GetKeyDown(KeyCode.LeftControl)) BoosterManager.dirIns.UseBooster();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint") GameManager.ManagerIns.CheckPoint();
        if (other.tag == "FinishLine") GameManager.ManagerIns.Goal();
        // 떨어지면 다시 처음 위치로 감
        if (other.tag == "Ground") transform.position = firstPos; 
    }

}
