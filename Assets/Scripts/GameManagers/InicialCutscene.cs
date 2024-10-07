using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Fade))]

public class PlayerInicialCutscene : MonoBehaviour
{
    [Header("NPC")]
    public string actorName;
    public CharacterCutsceneMovement shinobi;

    [Header("Player")]
    [SerializeField]private float playerSpeed = 4f;
    public float targetDistance;
    public Transform playerPos;
    public Vector3 offset; // Offset da câmera em relação ao jogador

    [Header("Camera")]
    private Transform camPos; // Transform da câmera
    private bool hasCameraMoved = false;
    private bool isCameraMoving = false;
    public float moveDuration = 1.5f; // Duração do movimento da câmera
    private Vector3 moveOffset = new Vector3(7.0f, 0f, 0f); // Offset de movimento da câmera


    private Fade fadeOut;
    private bool hasFaded = false;
    private DialogueManager dm;
    public CharacterCutsceneMovement player;

    public string[] initialDialogue;
    private bool hasDialogueStarted = false;  // Controle para garantir que o diálogo só inicie uma vez
    
    void Awake()
    {
        enabled = true;
    }

    void Start(){
        Time.timeScale = 1;  // Garantir que o tempo esteja fluindo normalmente
        dm = FindObjectOfType<DialogueManager>();
        camPos = Camera.main.transform;
        fadeOut = GetComponent<Fade>();
    }
    protected virtual void Update()  
    {
        MoveCamera(offset);
        if(dm.IsDialogueFinished()){//acabaram as falas do dialogo
            dm.audioSource.enabled = false;
            dm.EndDialogue();
            if(shinobi.HasReachedTargetDistance(3f)){
                shinobi.gameObject.SetActive(false);
                if (!hasFaded) {
                    hasFaded = true;  // Marca que o fade foi iniciado
                    enabled = false;
                    StartCoroutine(FadeOutToNextSceneWithDelay(2f));  // Inicia o fade após o delay
                }
            }else{
                if(!shinobi.isFacingRight)
                    shinobi.Flip();
                shinobi.Move(playerSpeed, 3f);
            }
        }
        if (player.HasReachedTargetDistance(targetDistance))  // Um método para verificar se o movimento terminou
        {
            player.StopMovement();
            // Inicia o diálogo apenas uma vez
            if (!hasDialogueStarted)
            {
                StartCoroutine(StartDialogueAfterDelay(0f));
                hasDialogueStarted = true;
            }else{//se o dialogo começou, entao o jogador pode skipar as frases
                if (Input.GetButtonDown("Jump"))
                {
                    if (dm.isTyping)
                    {
                        if(isCameraMoving){
                            offset = new Vector3(3.0f, 3.5f, -10f);
                            MoveCamera(offset);
                        }
                        StopAllCoroutines();
                        dm.typingSpeed = 0.001f;
                        dm.isTyping = false;
                    }
                    else
                    {
                        dm.typingSpeed = 0.05f;
                        if(!hasCameraMoved){
                            StopAllCoroutines();
                            StartCoroutine(MoveCameraCoroutine());
                            StartCoroutine(PlayNextLineAfterDelay());
                            hasCameraMoved = true; // Marca que o movimento foi concluído
                        }else{
                            dm.NextLine();
                        }
                        
                    }
                }
            }
        }
        else
        {
            player.Move(playerSpeed, targetDistance);  // Mover o jogador
        }
    }

    private IEnumerator StartDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Esperar pelo tempo definido (2 segundos)
        dm.StartDialogue(actorName, initialDialogue); // Iniciar o diálogo
    }
    private IEnumerator FadeOutToNextSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Espera o tempo de atraso (2 segundos neste caso)
        yield return fadeOut.FadeOutToBlackLoadNextScene("SampleScene");   // Inicia o fade out após o delay
    }

    void MoveCamera(Vector3 camOffset){
         // Calcula a posição desejada da câmera com base na posição do jogador e o offset
        Vector3 desiredPosition = playerPos.position + camOffset;

        

        // Atualiza a posição da câmera
        camPos.position = desiredPosition;
        
    }
    private IEnumerator MoveCameraCoroutine()
    {
        isCameraMoving = true;
        Vector3 startPosition = camPos.position;
        Vector3 targetPosition = startPosition + moveOffset;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            camPos.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camPos.position = targetPosition; // Assegura a posição final
        offset = new Vector3(3.0f, 3.5f, -10f);
        isCameraMoving = false;
    }

    private IEnumerator PlayNextLineAfterDelay(){
        yield return new WaitForSeconds(2f);
        dm.NextLine();
    }
}