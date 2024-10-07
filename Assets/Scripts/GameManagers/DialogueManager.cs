using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header ("Components")]
    public GameObject dialogueBox;
    public GameObject actorNameBox;
    public Text dialogueText;
    public Text actorNameText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typeSFX;
    private float lastAudioTime = 0f;    // Marca o tempo da última reprodução do som
    private float minAudioInterval = 0.05f;  // Intervalo mínimo entre toques do áudio

    public string[] dialogueLines; // Array de linhas do diálogo
    public bool isTyping = false;
    public bool dialogueFinished = false;
    public int currentLineIndex = 0;
    public float typingSpeed = 0.1f;

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }
    public void StartDialogue(string actorName, string[] lines){
        dialogueBox.SetActive(true);
        actorNameBox.SetActive(true);
        dialogueLines = lines;
        actorNameText.text = actorName;
        currentLineIndex = 0;
        dialogueFinished = false;
        StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

 

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            if (Time.time - lastAudioTime > minAudioInterval)// toca o audio em um intervalo minimo, caso o dialogo seja acelerado
            {
                audioSource.PlayOneShot(typeSFX);
                lastAudioTime = Time.time;  // Atualiza o tempo da última reprodução do som
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void NextLine()
    {
        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
        }
        else
        {
            dialogueFinished = true; // Marca o diálogo como finalizado
            Debug.Log("Fim do diálogo.");
        }
    }

    public void EndDialogue(){
        dialogueBox.SetActive(false);
        actorNameBox.SetActive(false);
    }
    public bool IsDialogueFinished()
    {
        return dialogueFinished;
    }
}
