using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BoardManager : MonoBehaviour
{
    public Text[] numTexts;        // 게시판 번호를 표시할 텍스트 배열
    public Text[] titleTexts;      // 게시판 제목을 표시할 텍스트 배열
    public Text[] userIdTexts;        // 게시판 유저를 표시할 텍스트 배열
    public Button[] buttonTexts;     // 게시판 이동할 버튼튼 배열
    public Button prevButton;      // 이전 페이지 버튼
    public Button nextButton;      // 다음 페이지 버튼
    public Text pageText;          // 현재 페이지 표시용 텍스트
    public int currentPage = 1;   // 현재 페이지
    public int totalPages = 1;    // 총 페이지 수

    string serverUrl = "https://192.168.20.38:3000/api/board";  // 서버 URL
    string countUrl = "https://192.168.20.38:3000/api/board/count"; // 총 게시글 수를 가져오는 API URL

    void Start()
    {
        prevButton.onClick.AddListener(() => ChangePage(-1));
        nextButton.onClick.AddListener(() => ChangePage(1));
        restartBoard();
    }

    // 페이지 변경
    void ChangePage(int pageDelta)
    {
        currentPage += pageDelta;

        // 페이지 범위 제한
        currentPage = Mathf.Clamp(currentPage, 1, totalPages);

        StartCoroutine(GetBoardData(currentPage));
        pageText.text = "Page " + currentPage + " / " + totalPages;
    }

    // 서버에서 게시판 데이터 가져오기
    public IEnumerator GetBoardData(int page)
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl + "?page=" + page);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSON 파싱
            string json = request.downloadHandler.text;
            Board[] boards = JsonUtility.FromJson<BoardList>("{\"boards\":" + json + "}").boards;

            // 4개의 게시글을 표시
            for (int i = 0; i < titleTexts.Length; i++)
            {
                if (i < boards.Length)
                {
                    titleTexts[i].text = boards[i].b_title;
                    numTexts[i].text = boards[i].b_num.ToString();  // 게시글 번호를 표시
                    userIdTexts[i].text = boards[i].id;
                    buttonTexts[i].interactable = true;
                }
                else
                {
                    // 게시글이 부족하면 빈 텍스트로 처리
                    titleTexts[i].text = "No more posts";
                    numTexts[i].text = "";  // 번호는 비우기
                    userIdTexts[i].text = "";
                    buttonTexts[i].interactable = false;
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
    public IEnumerator GetTotalPages()
    {
        UnityWebRequest request = UnityWebRequest.Get(countUrl);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            int totalCount = JsonUtility.FromJson<TotalCount>(json).count;

            // 총 페이지 수 계산 (한 페이지에 4개의 게시글 표시)
            totalPages = Mathf.CeilToInt((float)totalCount / 4f);
            pageText.text = "Page " + currentPage + " / " + totalPages;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void restartBoard(){
        currentPage = 1;
        StartCoroutine(GetTotalPages());
        StartCoroutine(GetBoardData(currentPage));
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
