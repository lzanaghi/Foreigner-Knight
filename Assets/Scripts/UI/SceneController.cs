using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        // Carrega a nova cena, substituindo a atual
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGame(){
        SceneManager.LoadScene("InicialCutscene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}