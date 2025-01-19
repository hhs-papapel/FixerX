using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BoardInfoManager : MonoBehaviour
{
    public Text upText;        // 게시판 위에를 표시할 텍스트
    public Text titleText;      // 게시판 제목을 표시할 텍스트
    public Text ContentText;        // 게시판 내용을 표시할 텍스트 배열

    string infoUrl = "https://192.168.20.38:3000/api/board/info"; // 게시글 info 데이터 가져오는 URL
    
    public GameObject NowObject; //현재 창
    public Button deleteButton; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 서버에서 게시판 데이터 가져오기
    public IEnumerator GetBoardData(int page)
    {
        UnityWebRequest request = UnityWebRequest.Get(infoUrl + "?page=" + page);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSON 파싱
            string json = request.downloadHandler.text;
            Board boards = JsonUtility.FromJson<BoardList>("{\"boards\":" + json + "}").boards;

            upText.text = boards.b_num.ToString()+". "+boards.id+"님이 "+boards.b_date+"에 작성하신 글";
            titleText.text = boards.b_title;
            ContentText.text = boards.b_content;
            
            deleteButton.interactable = (boards.id == GlobalUser.UserId);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    [System.Serializable]
    public class Board
    {
        public int b_num;
        public string u_idx;
        public string b_title;
        public string b_content;
        public string b_date;
        public string id;
    }

    [System.Serializable]
    public class BoardList
    {
        public Board boards;
    }

    // SSL 인증서 검증 비활성화 클래스
    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // 항상 true를 반환하여 SSL 인증서 검증을 무시
            return true;
        }
    }

    public void closetap(){
        NowObject.SetActive(false);
    }
}
