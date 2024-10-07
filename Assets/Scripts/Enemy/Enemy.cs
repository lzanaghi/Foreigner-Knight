using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;
    public bool isFacingRight;
    public bool canAttack;
    protected string order;
    protected string species; 
    public bool canTakeDamage = true;
    private float hitstopDuration = 0.1f;
    public bool isHitstopActive;
    public float attackCooldown = 1f;
    public Collider2D attackHitbox;
    private Coroutine deactivateHitboxCoroutine;
    public float detectionRange = 5.0f;
    public float attackRange;
    public int attackDamage;
    public Transform attackPoint;
    public Transform player;
    protected Enemy enemy;
    public AudioSource audioSource;  // Componente AudioSource
    public AudioClip[] hurtSounds;  // Array com os três efeitos sonoros
    public AudioClip deathSound;
    public AudioClip attack1Sound;
    public GameObject childObjectCollider;

     // Static variable to track if hitstop is active
    private static bool isGlobalHitstopActive = false;
    

    public Animator animator;
    public Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; 
        enemy = GetComponent<Enemy>();
        canAttack = true;

    }


    void Update()
    {
        Move();
        if(isFacingRight){
            if (player.position.x < transform.position.x){
                FlipEnemyScale();
            }
        }else{
            if (player.position.x > transform.position.x){
                FlipEnemyScale();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o jogador entrou na hitbox de ataque
        if (attackHitbox != null && attackHitbox.enabled && collision.CompareTag("Player"))
        {
            PlayerCombat player = collision.GetComponent<PlayerCombat>();
            if (player != null)
            {
                player.TakeDamage(attackDamage); // Dano causado ao jogador
            }
        }
    }

    public abstract IEnumerator Attack();

    public void TakeDamage(int damage){
        if (currentHealth <= 0)//nao faz nada se o inimigo esta morto
            return;
        if(canTakeDamage){
            currentHealth -= damage;

            //hurt animation
            Vector2 knockBackDirection = transform.position - player.position;
            knockBackDirection.y = 0;
            knockBackDirection.Normalize();
            float knockBackForce = 3;
            if(knockBackDirection.x < 0)
                knockBackForce = - knockBackForce;
            rig.velocity = Vector2.zero;
            rig.AddForce(new Vector2(knockBackForce, 3), ForceMode2D.Impulse);
            animator.SetTrigger("takeDmg");
            PlayHurtSound();
            //dead
            if(currentHealth <= 0){
                audioSource.PlayOneShot(deathSound);
                Die();   
            }
        }
    }

    void Die()
    {
        // Inicia a corrotina de hitstop se ainda não está ativa
        if (!isGlobalHitstopActive)
        {
            StartCoroutine(ApplyHitstop());
        }
        animator.SetBool("isDead", true);

        // Desabilita o inimigo
        gameObject.layer = 11;
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        // Desativa cada script
        Collider2D collider = childObjectCollider.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false; // Desativa o Collider
        }
        GoblinBombShot script = GetComponent<GoblinBombShot>();
        if (script != null)
        {
            script.enabled = false;
        }
        this.enabled = false;
    }
    
    public void Move(){
        if(Mathf.Abs(rig.velocity.x) >= 0.1f){
            animator.SetBool("run", true);
        }else{
            animator.SetBool("run", false);
        }
    }
    

    public void FlipEnemyScale()
    {
        // Inverter o valor de movingRight
        isFacingRight = !isFacingRight;

        // Inverter o localScale no eixo X
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        enemy.rig.velocity = Vector2.zero;
    }

    public void ActivateEnemyHitbox()// method activated on attack animation
    {
        // Ativa a hitbox
        attackHitbox.enabled = true;

        // Se já houver uma coroutine rodando para desativar a hitbox, cancela
        if (deactivateHitboxCoroutine != null)
        {
            StopCoroutine(deactivateHitboxCoroutine);
        }

        // Inicia a coroutine para garantir que a hitbox será desativada após um tempo, mesmo se a animação for interrompida
        deactivateHitboxCoroutine = StartCoroutine(DeactivateHitboxAfterTime(0.1f)); // 0.1f é o tempo em segundos
    }

    public void DeactivateEnemyHitbox()
    {
        // Desativa a hitbox
        attackHitbox.enabled = false;

        // Se a coroutine ainda estiver ativa, a cancela
        if (deactivateHitboxCoroutine != null)
        {
            StopCoroutine(deactivateHitboxCoroutine);
            deactivateHitboxCoroutine = null;
        }
    }

    private IEnumerator DeactivateHitboxAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Desativa a hitbox após o tempo especificado
        attackHitbox.enabled = false;
    }

    public void ActivateInvincibility(){
        canTakeDamage = false;
    }

    public void DeactivateInvincibility(){
        canTakeDamage = true;
    }

     public IEnumerator ApplyHitstop()
    {
        isGlobalHitstopActive = true;

        // Guarda o time scale original
        float originalTimeScale = Time.timeScale;

        // Define o time scale para um valor baixo
        Time.timeScale = 0.01f;

        // Espera pelo tempo do hitstop
        yield return new WaitForSecondsRealtime(hitstopDuration);

        // Restaura o time scale original
        Time.timeScale = originalTimeScale;

        isGlobalHitstopActive = false;
    }

    void PlayHurtSound()
    {
        if (hurtSounds == null || hurtSounds.Length == 0)
        {
            // O vetor é nulo ou está vazio, então não há sons para tocar.
            return;
        }
        int randomIndex = Random.Range(0, hurtSounds.Length);
        audioSource.PlayOneShot(hurtSounds[randomIndex]);
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    // Verify if the other object is an enemy
    if (collision.gameObject.CompareTag("Enemy"))
    {
        // get the collided enemy specie
        string otherEnemySpecies = collision.gameObject.GetComponent<Enemy>().species;

        // if same species ignore collision
        if (species != otherEnemySpecies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
