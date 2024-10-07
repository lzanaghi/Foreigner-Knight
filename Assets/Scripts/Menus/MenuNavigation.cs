using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [Header("SFX")]
    private AudioSource audioSource;
    public AudioClip navigateSFX;

    [Header("Buttons Logic")]
    public float inputDelay = 1f;  // Tempo entre entradas de navegação
    private float lastInputTime;     // Armazena o tempo da última entrada

    private int selectedIndex = 0;   // Índice do botão atualmente selecionado
    public Button[] menuButtons;     // Lista de botões no menu

    void Start(){
        audioSource = GetComponent<AudioSource>();
        ActivateMenu();
    }

    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            // Apenas navega se o tempo desde a última navegação for maior que o inputDelay
            if (Time.unscaledTime - lastInputTime > inputDelay)
            {
                // Usando os eixos do D-Pad e do analógico
                float vertical = Input.GetAxis("Vertical");
                float dpadVertical = Input.GetAxis("DPadVertical");

                // Verifica navegação para cima
                if (Input.GetKeyDown(KeyCode.UpArrow) || vertical > 0.5f || dpadVertical > 0.5f)
                {
                    Navigate(-1);  // Move para cima no menu
                    audioSource.PlayOneShot(navigateSFX, 0.3f);
                    lastInputTime = Time.unscaledTime;  // Atualiza o tempo da última navegação
                }
                // Verifica navegação para baixo
                else if (Input.GetKeyDown(KeyCode.DownArrow) || vertical < -0.5f || dpadVertical < -0.5f)
                {
                    Navigate(1);  // Move para baixo no menu
                    audioSource.PlayOneShot(navigateSFX, 0.3f);
                    lastInputTime = Time.unscaledTime;  // Atualiza o tempo da última navegação
                }
            }

            // Confirmar seleção
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
            {
                menuButtons[selectedIndex].onClick.Invoke();  // Aciona o botão selecionado
            }
        }
    }

    public void ActivateMenu()
    {
        // Seleciona o primeiro botão ao ativar o menu
        selectedIndex = 0;
        menuButtons[selectedIndex].Select();
    }

    private void Navigate(int direction)
    {
        // Deselect the current button
        menuButtons[selectedIndex].OnDeselect(null);

        //select new button index
        selectedIndex = (selectedIndex + direction + menuButtons.Length) % menuButtons.Length;
        menuButtons[selectedIndex].Select();
    }
}