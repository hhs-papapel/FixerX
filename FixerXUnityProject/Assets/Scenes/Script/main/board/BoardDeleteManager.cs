using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BoardDeleteManager : MonoBehaviour
{

    public Button deleteButton; // 게시글 삭제 버튼
    public Text postNumber; // 게시글 번호를 입력받는 필드
    public errPopupPageManager errpopup; //오류팝업 창
    public errPopupPageManager inpopup; //성공팝업 창
    public GameObject writeObject; // 현재 창 오브젝트트
    public BoardManager boardManager;

    string serverUrl = "https://192.168.20.38:3000/api/board/delete"; // Node.js 서버의 URL


    // Start is called before the first frame update
    void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    // 삭제 버튼 클릭 시 호출되는 메서드
    void OnDeleteButtonClicked()
    {
        
        if (string.IsNullOrEmpty(postNumber.text))
        {
            errpopup.popupOpen("게시글 번호 오류류!");
            return;
        }

        if (int.TryParse(postNumber.text, out int b_num)){
            // 삭제 요청 보내기
            StartCoroutine(DeletePost(b_num));
        }
        else{
            errpopup.popupOpen("잘못된 게시판 번호입니다!");
        }
    }

    // 게시글 삭제 요청을 서버에 보내는 코루틴
    IEnumerator DeletePost(int b_num)
    {
        // JSON 데이터로 요청 본문을 생성
        string jsonData = "{\"b_num\": " + b_num + "}";


        // UnityWebRequest로 DELETE 요청 보내기
        UnityWebRequest request = new UnityWebRequest(serverUrl, "DELETE");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        // 요청 시간 초과 설정
        request.timeout = 30;

        // 서버 요청
        Debug.Log("Before SendWebRequest");
        yield return request.SendWebRequest();
        Debug.Log("After SendWebRequest");

        // 서버 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Server Response: " + responseText);
            boardManager.restartBoard();
            writeObject.SetActive(false);

            // 성공 메시지 표시
            inpopup.popupOpen("게시글 삭제 성공!");

        }
        else
        {
            Debug.LogError("Error: " + request.error);
            errpopup.popupOpen("게시글 삭제 실패!");
        }
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

}
