using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScorePrint : MonoBehaviour
{
    public Text bestScoreTxt;
    float bestScore;
    string bestScoreStr;
    private void Start()
    {
        bestScore = PlayerPrefs.GetFloat("BestScore", 0f); // 최고점수 불러오기
        bestScoreStr = bestScore.ToString("00.00");
        bestScoreStr = bestScoreStr.Replace(".", ":");
        bestScoreTxt.text = bestScoreStr;
    }

}
