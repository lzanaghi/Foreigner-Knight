using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombat : Player
{
    public Animator animator;
    public Rigidbody2D rig;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 5;
    public LayerMask[] enemyLayers;
    public GhostingEffect ghost;
    public AudioSource audioSource;
    public AudioClip[] swordSounds;
    
    public bool canTakeDamage = true;
    private bool isAttacking = false;
    private float attackCooldown = 0.7f;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    private List<Collider2D> hitEnemies = new List<Collider2D>();
    public PlayerMovement playerMovement;
    public HealthBar healthBar;

    void Start(){
        currentHealth = maxHealth;
    }

    void Update()
    {
        if(isAttacking){
            playerMovement.canRoll = false;
            ghost.makeGhosting = true;
            return;
        }else{
            playerMovement.canRoll = true;
            if(Time.time >= nextAttackTime){
                if (Input.GetKeyDown(KeyCode.Z) && canAttack)
                {
                    StartCoroutine(Attack());
                    nextAttackTime = Time.time +1f / attackRate;

                }
            }
        }
    }

    IEnumerator Attack()
    {
        // Inicia a animação de ataque
        animator.SetTrigger("attack1Movement");
        PlayRandomSwordSound();
        yield return new WaitForSeconds(attackCooldown);
        hitEnemies.Clear(); // Limpa a lista de inimigos atingidos
    }

    public void ActivateHitbox()
    {
        // Ativa o estado de ataque
        isAttacking = true;
    }

    public void DeactivateHitbox()
    {
        // Desativa o estado de ataque
        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            // Detecta inimigos na área de ataque durante o período da hitbox ativa
            Collider2D[] detectedEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers[0]);
            Collider2D[] detectedEnemiesNoCollision = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers[1]);
            Collider2D[] detectedEnemiesTotal = detectedEnemies.Concat(detectedEnemiesNoCollision).ToArray();

            foreach (Collider2D enemy in detectedEnemiesTotal)
            {   
                if (!hitEnemies.Contains(enemy))
                {
                    // Aplica o dano apenas se o inimigo ainda não foi atingido durante este ataque

                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                    hitEnemies.Add(enemy); // Adiciona o inimigo à lista de atingidos
                }
            }
        }
    }

    public void TakeDamage(int damage){
        if(canTakeDamage){
            healthBar.delayTime = healthBar.MAX_DELAY_TIME;
            currentHealth -= damage;

            //hurt animation
            DeactivateHitbox();
            animator.SetTrigger("player_hurt");
            rig.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);

            //dead
            if(currentHealth <= 0){
                //die  
                rig.velocity = new Vector3(0f,0f,0f);
                GetComponent<Player>().enabled = false;
                enabled = false;
            }
        }
            
    }

    void PlayRandomSwordSound()
    {
        int randomIndex = Random.Range(0, swordSounds.Length);
        audioSource.PlayOneShot(swordSounds[randomIndex]);
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}