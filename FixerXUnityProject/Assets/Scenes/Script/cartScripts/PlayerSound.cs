using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    PlayerController player;
    public AudioSource driveSource;
    public AudioSource idleSource;
    public AudioSource driftSource;
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // 음악 볼륨으로 음악 꺼짐/켜짐 구분
    public void Playsound()
    {
        if (player.playerState == PlayerController.PlayerState.Drive)
        {
            driveSource.volume = 1;
            idleSource.volume = 0;
            driftSource.volume = 0;
        }
        else if (player.playerState == PlayerController.PlayerState.Idle)
        {
            idleSource.volume = 1;
            driveSource.volume = 0;
            driftSource.volume = 0;
        }

        if (player.isShift)
        {
            idleSource.volume = 0;
            driveSource.volume = 0;
            driftSource.volume = 1;
        }

    }
    
}
