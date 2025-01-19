using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardWriteManager : MonoBehaviour
{
    
    public InputField titleText; // 게시판 제목 입력 필드
    public InputField contentText; // 게시판 내용 입력 필드
    
    public errPopupPageManager errpopup; //오류팝업 창
    public errPopupPageManager inpopup; //성공팝업 창
    public GameObject writeObject; // 현재 창 오브젝트트
    public BoardManager boardManager;

    // 서버 URL (Node.js의 로그인 엔드포인트)
    private string serverUrl = "https://192.168.20.38:3000/api/board/write";

    // Unity에서 게시즐 작성성 요청 처리
    public IEnumerator boardWrite(string id, string title, string content)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new BoardWriteRequest(id, title,content));

        // UnityWebRequest 설정
        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        // 서버 요청
        yield return request.SendWebRequest();

        // 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("wirte successful: " + request.downloadHandler.text);
 
            // 서버 응답 처리
            try
            {
                string responseText = request.downloadHandler.text;
                TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);
                // Debug.Log("JWT Token: " + tokenResponse.token);

                // // 서버 응답의 메시지 출력
                // Debug.Log("Server Message: " + tokenResponse.message);
                titleText.text = "";
                contentText.text = "";
                writeObject.SetActive(false);
                boardManager.restartBoard();
                inpopup.popupOpen("게시글이 작성되었습니다!");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing response: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("등록 실패");
        }
    }

    // JSON 요청 구조체
    [System.Serializable]
    public class BoardWriteRequest
    {
        public string id;
        public string title;
        public string content;

        public BoardWriteRequest(string id, string title, string content)
        {
            this.id = id;
            this.title = title;
            this.content = content;
        }
    }

    // JSON 응답 구조체
    [System.Serializable]
    public class TokenResponse
    {
        public string message;
        public string token;
    }

    // 버튼 클릭 이벤트 연결 메서드
    public void WriteEvent()
    {
        if(string.IsNullOrEmpty(titleText.text)){
            errpopup.popupOpen("제목을을 입력해주세요!");
            return;
        }
        
        if(string.IsNullOrEmpty(contentText.text)){
            errpopup.popupOpen("내용을을 입력해주세요!");
            return;
        }
        

        if(titleText.text.Length < 3 || titleText.text.Length >= 20){
            errpopup.popupOpen("제목은 3글자 이상 20글자 이하까지 가능합니다!");
            return;
        }
        
        if(contentText.text.Length < 3 || contentText.text.Length >= 200){
            errpopup.popupOpen("내용은 3글자 이상 200글자 이하까지 가능합니다!");
            return;
        }

        StartCoroutine(boardWrite(GlobalUser.UserId, titleText.text, contentText.text));
        
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

    public void closeWrite(){
        titleText.text = "";
        contentText.text = "";
        writeObject.SetActive(false);
    }
    public void openWrite(){
        writeObject.SetActive(true);
    }
}
