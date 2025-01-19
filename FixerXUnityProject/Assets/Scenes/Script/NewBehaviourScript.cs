using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    
    public GameObject aObject; 

    public void gameStartOpen(){
        aObject.SetActive(true);
    }
}
