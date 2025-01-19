using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardCommentManger : MonoBehaviour
{
    
    [Header("댓글글 불러오기")]
    public Text[] dateTexts;        // 게시판 번호를 표시할 텍스트 배열
    public Text[] titleTexts;      // 게시판 제목을 표시할 텍스트 배열
    public Text[] userIdTexts;        // 게시판 유저를 표시할 텍스트 배열
    public Button prevButton;      // 이전 페이지 버튼
    public Button nextButton;      // 다음 페이지 버튼
    public Text pageText;          // 현재 페이지 표시용 텍스트
    public Text boardNum;
    public int currentPage = 1;   // 현재 페이지
    public int totalPages = 1;    // 총 페이지 수


    [Header("댓글글 등록")]
    public InputField contentText; // 게시판 내용 입력 필드
    public errPopupPageManager errpopup; //오류팝업 창
    public errPopupPageManager inpopup; //성공팝업 창

    string serverUrl = "https://192.168.20.38:3000/api/board/comment";  // 서버 URL
    string countUrl = "https://192.168.20.38:3000/api/board/comment/count"; // 총 게시글 수를 가져오는 API URL
    private string serverUrl2 = "https://192.168.20.38:3000/api/board/comment/write";

    void Start()
    {
        prevButton.onClick.AddListener(() => ChangePage(-1));
        nextButton.onClick.AddListener(() => ChangePage(1));
    }

    // 페이지 변경
    void ChangePage(int pageDelta)
    {
        currentPage += pageDelta;

        // 페이지 범위 제한
        currentPage = Mathf.Clamp(currentPage, 1, totalPages);
        if (int.TryParse(boardNum.text, out int b_num)){
            StartCoroutine(GetBoardData(currentPage, b_num));
        }
        else{
            errpopup.popupOpen("잘못된 게시판 번호입니다!");
        }
        pageText.text = "Page " + currentPage + " / " + totalPages;
    }

    // 서버에서 게시판 데이터 가져오기
    public IEnumerator GetBoardData(int page, int b_num)
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl + "?page=" + page + "&b_num=" + b_num);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSON 파싱
            string json = request.downloadHandler.text;
            Board[] boards = JsonUtility.FromJson<BoardList>("{\"boards\":" + json + "}").boards;

            // 3개의 게시글을 표시
            for (int i = 0; i < titleTexts.Length; i++)
            {
                if (i < boards.Length)
                {
                    titleTexts[i].text = boards[i].c_content;
                    dateTexts[i].text = boards[i].b_date;
                    userIdTexts[i].text = boards[i].id;
                }
                else
                {
                    // 게시글이 부족하면 빈 텍스트로 처리
                    titleTexts[i].text = "No more posts";
                    dateTexts[i].text = "";
                    userIdTexts[i].text = "";
                }
            }

            // 버튼 활성화/비활성화 처리
            prevButton.interactable = currentPage > 1;
            nextButton.interactable = currentPage < totalPages;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    // 총 페이지 수 가져오기
    public IEnumerator GetTotalPages(int b_num)
    {
        UnityWebRequest request = UnityWebRequest.Get(countUrl+"?b_num="+b_num);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            int totalCount = JsonUtility.FromJson<TotalCount>(json).count;

            // 총 페이지 수 계산 (한 페이지에 3개의 게시글 표시)
            totalPages = Mathf.CeilToInt((float)totalCount / 3f);
            pageText.text = "Page " + currentPage + " / " + totalPages;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void restartBoard(){
        currentPage = 1;
        if (int.TryParse(boardNum.text, out int b_num)){
            StartCoroutine(GetTotalPages(b_num));
            StartCoroutine(GetBoardData(currentPage,b_num));
        }
        else{
            errpopup.popupOpen("잘못된 게시판 번호입니다!");
        }
    }
    

    // Unity에서 댓글 작성성 요청 처리
    public IEnumerator boardWrite(string id, int b_num, string content)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new BoardWriteRequest(id, b_num,content));

        // UnityWebRequest 설정
        UnityWebRequest request = new UnityWebRequest(serverUrl2, "POST");
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
                contentText.text = "";
                restartBoard();
                inpopup.popupOpen("댓글글이 작성되었습니다!");
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
        public int b_num;
        public string content;

        public BoardWriteRequest(string id, int b_num, string content)
        {
            this.id = id;
            this.b_num = b_num;
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
        if(string.IsNullOrEmpty(contentText.text)){
            errpopup.popupOpen("내용을 입력해주세요!");
            return;
        }
        
        if(contentText.text.Length < 3 || contentText.text.Length >= 20){
            errpopup.popupOpen("내용은 3글자 이상 20글자 이하까지 가능합니다!");
            return;
        }

        if (int.TryParse(boardNum.text, out int b_num)){
            StartCoroutine(boardWrite(GlobalUser.UserId, b_num, contentText.text));
        }
        else{
            errpopup.popupOpen("잘못된 게시판 번호입니다!");
        }
    }



    [System.Serializable]
    public class Board
    {
        public int c_num;
        public string u_idx;
        public int b_num;
        public string c_content;
        public string b_date;
        public string id;
    }

    [System.Serializable]
    public class BoardList
    {
        public Board[] boards;
    }

    [System.Serializable]
    public class TotalCount
    {
        public int count;
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
