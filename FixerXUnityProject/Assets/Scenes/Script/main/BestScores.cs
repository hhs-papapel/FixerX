using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BestScores : MonoBehaviour
{
    public Text[] rankTexts;        // 순위를 표시할 텍스트 배열

    string bestScoreUrl = "https://192.168.20.38:3000/api/raceScore/bestscore"; // 레이싱 최고 점수 API URL

    void Start()
    {
        StartCoroutine(GetBestScores());
    }

    // 서버에서 레이싱 최고 점수 가져오기
    public IEnumerator GetBestScores()
    {
        UnityWebRequest request = UnityWebRequest.Get(bestScoreUrl);

        // SSL 인증서 검증 비활성화
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // JSON 파싱
            string json = request.downloadHandler.text;
            RaceScore[] scores = JsonUtility.FromJson<RaceScoreList>("{\"scores\":" + json + "}").scores;

            // 데이터를 UI에 표시
            for (int i = 0; i < rankTexts.Length; i++)
            {
                if (i < scores.Length)
                {
                    string mapname = "";
                    if(scores[i].r_map == "1"){
                        mapname = "봄";
                    }else if(scores[i].r_map == "2"){
                        mapname = "겨울";
                    }else if(scores[i].r_map == "3"){
                        mapname = "해변";
                    }else if(scores[i].r_map == "4"){
                        mapname = "사막";
                    }else if(scores[i].r_map == "5"){
                        mapname = "도시";
                    }
                    rankTexts[i].text = ""+scores[i].id+" | "+mapname+" | "+scores[i].r_time;  
                }
                else
                {
                    // 순위가 부족한 경우 빈 데이터로 처리
                    rankTexts[i].text = "No Rank";
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }


    [System.Serializable]
    public class RaceScore
    {
        public string id;    // 유저 ID
        public string r_map; // 맵 정보 (필요에 따라 UI에 표시 가능)
        public string r_time; // 기록 시간
    }

    [System.Serializable]
    public class RaceScoreList
    {
        public RaceScore[] scores;
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
