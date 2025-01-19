using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public void GoToGameScene1()
    {
        SceneManager.LoadScene("Map1");
    }
    public void GoToGameScene2()
    {
        SceneManager.LoadScene("Map2");
    }

    public void GoToGameScene3()
    {
        SceneManager.LoadScene("Map3");
    }

    public void GoToGameScene4()
    {
        SceneManager.LoadScene("Map4");
    }

    public void GoToGameScene5()
    {
        SceneManager.LoadScene("Map5");
    }


    public void Exit()
    {
        Application.Quit();
    }

    public void GoToMain()
    {
        SceneManager.LoadScene("mainScene");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("StartScene");
    }
}
