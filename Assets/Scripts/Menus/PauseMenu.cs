using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip unpauseSFX;
    public AudioClip selectSFX;

    [Header("Components")]
    public static bool GameIsPaused = false;  // Estado do jogo (pausado ou não)
    public GameObject pauseMenuUI;            // Referência ao menu de pausa no Canvas
    private MenuNavigation menuNavigation;    // Referência à classe MenuNavigation

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        menuNavigation = GetComponent<MenuNavigation>(); // Pega a referência à classe MenuNavigation
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Detectar o botão de pausa (Esc no teclado ou Start no controle)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        // Desativa o menu de pausa e retoma o jogo
        audioSource.PlayOneShot(unpauseSFX, 0.25f);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;   // Retorna o tempo ao normal
        GameIsPaused = false;
    }

    public void ResumeOnClick()
    {
        // Desativa o menu de pausa e retoma o jogo
        audioSource.PlayOneShot(selectSFX, 0.25f);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;   // Retorna o tempo ao normal
        GameIsPaused = false;
    }

    void Pause()
    {
        // Ativa o menu de pausa e congela o jogo
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;   // Congela o jogo
        GameIsPaused = true;
        
        // Inicia a navegação pelo menu
        if (menuNavigation != null)
        {
            menuNavigation.ActivateMenu();
        }
    }

    public void ReturnToTitle(){
        audioSource.PlayOneShot(selectSFX, 0.25f);
        SceneManager.LoadScene("TitleScreen");
    }

    // Método para sair do jogo (pode ser adicionado ao botão de saída)
    public void QuitGame()
    {
        audioSource.PlayOneShot(selectSFX, 0.25f);
        Application.Quit();
    }
}