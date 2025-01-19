using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NodeJsClient : MonoBehaviour
{
    private string serverUrl = "http://localhost:3000/api/data";

    // Sending data to the server
    public IEnumerator SendDataToServer(string userId, int score)
    {
        string jsonData = JsonUtility.ToJson(new { userId = userId, score = score });

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data sent successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
        }
    }

    // Receiving data from the server
    public IEnumerator GetDataFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(serverUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data received: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error receiving data: " + request.error);
        }
    }

    private void Start()
    {
        // Sending data example
        StartCoroutine(SendDataToServer("Player1", 100));

        // Receiving data example
        StartCoroutine(GetDataFromServer());
    }
}
