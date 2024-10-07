using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MushroomAberration : Enemy
{
    public float moveSpeed = 2f; // Velocidade de movimento do inimigo
    public AudioSource attackAudioSource;

    private void Start(){
        currentHealth = maxHealth; 
        order = "Fungal";
        species = "Mushroom Aberration";
        enemy = GetComponent<Enemy>();
        canAttack = true;
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }
    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer < attackRange){
            StartCoroutine(Attack());
        }
        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            Chase();   
        }
    }
    public override IEnumerator Attack(){
            if(canAttack){
            // plays attack animation
            canAttack = false;
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }
    }
    public void Chase(){
        Vector2 direction = (player.position - transform.position).normalized;
        if(rig.velocity.y == 0)
            rig.velocity = new Vector2(direction.x * moveSpeed, enemy.rig.velocity.y);  
  
    }

    IEnumerator PlayAttack1Sound()
    {
        if (attack1Sound == null)
        {
            yield return null;
        }
        attackAudioSource.PlayOneShot(attack1Sound);
    }

}

