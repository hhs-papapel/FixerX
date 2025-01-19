using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class JoinManager : MonoBehaviour
{
    
    public InputField nameText; // 사용자의 PW 입력 필드
    public InputField idText; // 사용자의 ID 입력 필드
    public InputField pwText; // 사용자의 PW 입력 필드
    public InputField emailText; // 사용자의 PW 입력 필드
    
    public errPopupPageManager errpopup; //오류팝업 창
    public errPopupPageManager inpopup; //성공팝업 창
    public GameObject JoinObject; //회원가입 창
    

    // 서버 URL (Node.js의 로그인 엔드포인트)
    private string serverUrl = "https://192.168.20.38:3000/api/register";

    // Unity에서 로그인 요청 처리
    public IEnumerator Join(string username, string id, string password, string email)
    {
        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(new JoinRequest(username, id, password, email));

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
            Debug.Log("Join successful: " + request.downloadHandler.text);
 
            // 서버 응답 처리
            try
            {
                string responseText = request.downloadHandler.text;
                TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);
                // Debug.Log("JWT Token: " + tokenResponse.token);

                // // 서버 응답의 메시지 출력
                // Debug.Log("Server Message: " + tokenResponse.message);
                JoinObject.SetActive(false);
                inpopup.popupOpen("회원가입에 성공하셨습니다!");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing response: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("회원가입 실패");
        }
    }

    // JSON 요청 구조체
    [System.Serializable]
    public class JoinRequest
    {
        public string username;
        public string id;
        public string password;
        public string email;

        public JoinRequest(string username, string id, string password, string email)
        {
            this.username = username;
            this.id = id;
            this.password = password;
            this.email = email;
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
    public void JoinEvent()
    {
        if(string.IsNullOrEmpty(nameText.text)){
            errpopup.popupOpen("이름을 입력해주세요!");
            return;
        }
        
        if(idText.text == ""){
            errpopup.popupOpen("아이디를 입력해주세요!");
            return;
        }
        
        if(pwText.text == ""){
            errpopup.popupOpen("패스워드를 입력해주세요!");
            return;
        }
        
        if(emailText.text == ""){
            errpopup.popupOpen("이메일을 입력해주세요!");
            return;
        }

        if(idText.text.Length < 3 || idText.text.Length >= 10){
            errpopup.popupOpen("아이디는 3글자 이상 10글자 이하까지 가능합니다!");
            return;
        }

        if(pwText.text.Length < 3 ){
            errpopup.popupOpen("패스워드는 3글자 이상 부터터 가능합니다!");
            return;
        }

        string passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@#$%^&]).+$";

        if (!Regex.IsMatch(pwText.text, passwordPattern))
        {
            errpopup.popupOpen("패스워드는 영문, 숫자, 특수문자(@#$%^&)를 조합해야 합니다!");
            return;
        }
        // 이메일 정규식
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if(!Regex.IsMatch(emailText.text, emailPattern)){
            errpopup.popupOpen("유효하지 않는 이메일 입니다!");
            return;
        }

        StartCoroutine(Join(nameText.text, idText.text, pwText.text, emailText.text));
        
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

    public void JoinOff(){
        nameText.text = "";
        idText.text = "";
        pwText.text = "";
        emailText.text = "";

        JoinObject.SetActive(false);
    }

}
