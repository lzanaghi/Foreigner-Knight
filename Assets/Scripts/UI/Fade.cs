using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image fadeImage;          // Referência à Image de fade
    public float fadeDuration = 2f;  // Duração do fade em segundos
    private bool isFading = false;

    void Start()
    {
        // Inicializa com a imagem completamente transparente (se for um fade-in posterior)
        fadeImage.color = new Color(0f, 0f, 0f, 0f); 
    }

    // Coroutine para realizar o fade out
    public IEnumerator FadeOutToBlack()
    {
        isFading = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Calcula a interpolação da transparência
            fadeImage.color = new Color(0f, 0f, 0f, alpha); // Altera a opacidade
            yield return null; // Espera até o próximo frame
        }

        // Garantir que o fade termine com a imagem completamente opaca
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        isFading = false;
    }

    public IEnumerator FadeOutToBlackLoadNextScene(string nextScene)
    {
        isFading = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // Calcula a interpolação da transparência
            fadeImage.color = new Color(0f, 0f, 0f, alpha); // Altera a opacidade
            yield return null; // Espera até o próximo frame
        }

        // Garantir que o fade termine com a imagem completamente opaca
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        isFading = false;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(nextScene);
    }

    public IEnumerator FadeInFromBlack()
    {
        // Começa com a tela completamente preta
        fadeImage.color = new Color(0f, 0f, 0f, 1f);  // Configura a tela preta antes do fade-in
        isFading = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration)); // Reduz a opacidade de preto para transparente
            fadeImage.color = new Color(0f, 0f, 0f, alpha); // Clareia a tela gradualmente
            yield return null; // Espera até o próximo frame
        }

        // Garante que a imagem termine completamente transparente
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        isFading = false;
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeInFromBlack());
    }
}