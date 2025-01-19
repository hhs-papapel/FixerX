using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class cartBestScoreManager : MonoBehaviour
{
    public Text BestScoreTxt;
    public int mapNo;
    string countUrl = "https://192.168.20.38:3000/api/raceScore/mybest"; 
    
    
    void Start()
    {
        Bestscore();
    }

    // 최고기록 가져오기
    public IEnumerator GetBestScore(string id, int r_map)
    {
        UnityWebRequest request = UnityWebRequest.Get(countUrl+"?id="+id+"&r_map="+r_map);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            BestScoreTxt.text = JsonUtility.FromJson<BestS>(json).bestTime;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void Bestscore()
    {
        StartCoroutine(GetBestScore(GlobalUser.UserId, mapNo));
    }


    [System.Serializable]
    public class BestS
    {
        public string bestTime;
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
