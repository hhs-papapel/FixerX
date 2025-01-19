using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class errPopupPageManager : MonoBehaviour
{
    public GameObject errPopupObject;
    public Text errText;
    

    public void popupClose(){
        errPopupObject.SetActive(false);
    }

    public void popupOpen(string text){
        errText.text = text;
        errPopupObject.SetActive(true);
    }
}
