using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager dirIns = null; // 싱글턴 객체(GameDirector 대표 객체)

    GameObject boosterImage1;
    GameObject boosterImage2;
    GameObject boosterGage;
    GameObject player;
    AudioSource adio;

    void Awake()
    {
        if (dirIns == null) dirIns = this;
    }

    void Start()
    {
        boosterGage = GameObject.Find("Canvas").transform.Find("BoosterBar").transform.Find("boosterGage").gameObject; 
        boosterImage1 = GameObject.Find("Canvas").transform.Find("boosterImage1").gameObject;
        boosterImage2 = GameObject.Find("Canvas").transform.Find("boosterImage2").gameObject;
        player = GameObject.Find("Player");
        adio = gameObject.GetComponent<AudioSource>();
    }

    public void IncreaseBoosterGage()
    {
        boosterGage.GetComponent<Image>().fillAmount += 0.004f;
    }

    public void UseBooster()
    {
        if (boosterImage1.activeSelf)
        {
            if (boosterImage2.activeSelf)
            {
                boosterImage2.SetActive(false);
                boosterImage1.SetActive(true);
            }
            else
            {
                boosterImage1.SetActive(false);
            }
            StartCoroutine("BoosterOn");
        }
    }

    IEnumerator BoosterOn()
    {
        player.GetComponent<PlayerController>().moveSpeed += 3f;
        player.transform.Find("boosterEffect").gameObject.SetActive(true);
        adio.Play();
        yield return new WaitForSeconds(3);// 3초동안 정지
        player.GetComponent<PlayerController>().moveSpeed -= 3f;
        player.transform.Find("boosterEffect").gameObject.SetActive(false);
    }

    void Update()
    {
        if (boosterGage.GetComponent<Image>().fillAmount == 1)
        {
            if (boosterImage1.activeSelf) boosterImage2.SetActive(true);
            else boosterImage1.SetActive(true);
            
            boosterGage.GetComponent<Image>().fillAmount = 0f;
        }
    }
}
