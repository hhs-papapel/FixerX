using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class cartScoreManager : MonoBehaviour
{
    public Text timerText;
    public int mapNo;

    // 서버 URL (Node.js의 로그인 엔드포인트)
    string serverUrl = "https://192.168.20.38:3000/api/raceScore";

    // Unity에서 게시즐 작성성 요청 처리
    public IEnumerator raceScoreWrite(string id, int r_map, string r_time)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new BoardWriteRequest(id, r_map,r_time));

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
        public int r_map;
        public string r_time;

        public BoardWriteRequest(string id, int r_map, string r_time)
        {
            this.id = id;
            this.r_map = r_map;
            this.r_time = r_time;
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
    public void RaceScoreEvent()
    {
        StartCoroutine(raceScoreWrite(GlobalUser.UserId, mapNo, timerText.text));
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
