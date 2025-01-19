using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainLoad : MonoBehaviour
{
    public Text useridText;
    // Start is called before the first frame update
    void Start()
    {
        useridText.text = GlobalUser.UserId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
