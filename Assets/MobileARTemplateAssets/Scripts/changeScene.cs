using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    public void LoadScene1()
    {
        SceneManager.LoadScene("Scene_Dragon");
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene("carScene");
    }

    public void LoadDefaultScene()
    {
        SceneManager.LoadScene("ScenePrime");
    }

    public void LoadScene3()
    {
        SceneManager.LoadScene("ImageTrackingScene");
    }

    public void LoadScene4()
    {
        SceneManager.LoadScene("PointCloudScene");
    }

    public void LoadScene5()
    {
        SceneManager.LoadScene("BallShootingGameScene");
    }
    
    public void exitApp()
    {
        Application.Quit();
    }


}
