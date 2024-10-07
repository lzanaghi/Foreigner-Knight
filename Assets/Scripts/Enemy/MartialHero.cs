using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;


public class  MartialHero : Enemy
{

    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 4.0f;

    public bool isAttacking = false;

    public Transform[] patrolPoints;
    private int currentPatrolIndex;

    public GhostingEffect ghost;
    public LayerMask playerLayer;

    private void Start()
    {
        currentHealth = maxHealth;
        order = "Primate";
        species = "Human";
        canTakeDamage = true;
        enemy = GetComponent<Enemy>();
        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
        StartCoroutine(Patrol());
    }

    private void FixedUpdate()
    {
        if (!isAttacking && canAttack ) // Apenas detecta o jogador se não estiver atacando
            {
  
                DetectPlayer();
            }
    }
    

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
            {
                StopAllCoroutines();
                StartCoroutine(Attack());
            }
        else if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
        else
        {
            if (!IsInvoking("Patrol"))
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {   
            ghost.makeGhosting = false;
            Transform targetPoint = patrolPoints[currentPatrolIndex];
            //while (Vector2.Distance(transform.position, targetPoint.position) > 0f)
            //{   
                //enemy.rig.velocity = new Vector2((targetPoint.position - transform.position).normalized.x * patrolSpeed, enemy.rig.velocity.y);
                //yield return null;
            //}

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            yield return new WaitForSeconds(2.0f); // Pausa antes de ir para o próximo ponto de patrulha
        }
    }

    private IEnumerator ChasePlayer()
    {          
        Vector2 direction = player.position - transform.position;
        enemy.rig.velocity = new Vector2(direction.x *chaseSpeed, enemy.rig.velocity.y);
        ghost.makeGhosting = true;
        yield return new WaitForSeconds(10f);

        ghost.makeGhosting = false;
        enemy.rig.velocity = Vector2.zero;
    }

    public override IEnumerator Attack()
    {
        if(canAttack){
            canTakeDamage = false;
            // plays attack animation
            canAttack = false;
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(attackCooldown);
            canTakeDamage = true;
            canAttack = true;

        }
    }
    
}