using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCutsceneMovement : MonoBehaviour
{
    protected Rigidbody2D rb;            // Protegido para permitir acesso nas classes filhas
    protected Animator animator;         // Protegido para permitir acesso nas classes filhas
    public string nextScene;

    public bool isFacingRight = true; 
    protected bool isthisScriptRunning = false;
    protected float distanceMoved = 0f;  // Distância já percorrida
    private Vector3 initialPosition;

    // Start is chamado apenas uma vez no início
    protected virtual void Start()       // 'virtual' permite que classes filhas substituam este método
    {
        isthisScriptRunning = true;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D não encontrado no objeto " + gameObject.name);
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no objeto " + gameObject.name);
        }

        initialPosition = transform.position;
    }

    // FixedUpdate é chamado em intervalos fixos de tempo
    protected virtual void FixedUpdate()  // 'virtual' permite que classes filhas substituam este método
    {
       
    }

    // Método de movimento
    public void Move(float moveSpeed, float targetDistance)
    {
        // Verifica se o personagem já percorreu a distância alvo
        if (distanceMoved < targetDistance)
        {
            // Aplica velocidade ao Rigidbody no eixo X
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

            // Calcula a distância total percorrida desde o início
            distanceMoved = Vector3.Distance(initialPosition, transform.position);
        }

        // Define a animação com base no movimento
        if (Mathf.Abs(rb.velocity.x) >= 0.1f)
        {
            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }
    }

    public bool HasReachedTargetDistance(float targetDistance)
    {
        return distanceMoved >= targetDistance;
    }

    public void StopMovement()
    {
        rb.velocity = Vector2.zero; // Zera a velocidade ao atingir a distância
        animator.SetBool("run", false); // Para a animação de correr
    }

    // Método para carregar a próxima cena
    public void EndCutscene(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight; // Inverte a direção
        Vector3 scale = transform.localScale;
        scale.x *= -1;              // Inverte o valor do eixo X da escala
        transform.localScale = scale;
    }
}