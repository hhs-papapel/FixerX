using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{

    public InputField idText; // 사용자의 ID 입력 필드
    public InputField pwText; // 사용자의 PW 입력 필드
    
    public GameObject JoinObject; //회원가입 창
    public errPopupPageManager errpopup; //오류팝업 창
    public GameObject LogingObject; //로딩 창

    // 서버 URL (Node.js의 로그인 엔드포인트)
    private string serverUrl = "https://192.168.20.38:3000/api/login";

    // Unity에서 로그인 요청 처리
    public IEnumerator Login(string id, string password)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new LoginRequest(id, password));

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
            //Debug.Log("Login successful: " + request.downloadHandler.text);

            // 서버 응답 처리
            try
            {
                string responseText = request.downloadHandler.text;
                TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);
                // Debug.Log("JWT Token: " + tokenResponse.token);

                // // 서버 응답의 메시지 출력
                // Debug.Log("Server Message: " + tokenResponse.message);

                GlobalUser.UserId = idText.text;        // 사용자 ID 저장
                LogingObject.SetActive(true);

                StartCoroutine(LoadMainSceneAfterDelay(5f));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing response: " + e.Message);
            }
        }
        else
        {
            errpopup.popupOpen("아이디나 패스워드가 일치하지 않습니다.");
            Debug.LogError("로그인 실패");
        }
    }

    // 5초 후 씬 전환 메서드
    private IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("mainScene"); // mainScene으로 전환
    }

    // JSON 요청 구조체
    [System.Serializable]
    public class LoginRequest
    {
        public string id;
        public string password;

        public LoginRequest(string id, string password)
        {
            this.id = id;
            this.password = password;
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
    public void LoginEvent()
    {
        StartCoroutine(Login(idText.text, pwText.text));
    }

    public void JoinOn(){
        JoinObject.SetActive(true);
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
