using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        // Verifica se já existe uma instância do GameManager
        if (instance == null)
        {
            instance = this;
            Time.timeScale = 1;
            DontDestroyOnLoad(gameObject); // Mantém o GameManager ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // Destrói a nova instância se já existir uma
        }
    }

    private void Start()
    {
        // Verifique se a cena atual é a TitleScreen
        if (SceneManager.GetActiveScene().name != "TitleScreen")
        {
            // Se não estiver na TitleScreen, carregue-a
            SceneManager.LoadScene("TitleScreen");
        }
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}