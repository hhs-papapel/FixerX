using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BoardInfo : MonoBehaviour
{
    public Text num;
    public GameObject infoObject;
    public BoardInfoManager boardInfo;
    public BoardCommentManger boardComment;

    public Text boardNumText;

    public void boardoneClick(){
        boardNumText.text = num.text;
        infoObject.SetActive(true);
        boardInfo.StartCoroutine(boardInfo.GetBoardData(int.Parse(num.text)));
        boardComment.restartBoard();
    }
}
