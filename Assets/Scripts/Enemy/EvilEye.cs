using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilEye : Enemy
{
    [SerializeField]private float shotRange = 10f;
    [SerializeField]private float timer = 5f;
    [SerializeField]private float moveSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; 
        order = "Devil";
        species = "Eye Devil";
        enemy = GetComponent<Enemy>();
        canAttack = true;
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            StartCoroutine(Attack());
        }

        if (distanceToPlayer < detectionRange && distanceToPlayer < shotRange)
        {

            timer += Time.deltaTime;
            //logica do tiro
            
        }
        else
        {
            // Reset everything if the player is out of range
            timer = 0f;
        }

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            Chase();   
        }
    }

    public override IEnumerator Attack(){
        yield return new WaitForSeconds(attackCooldown);
    }

    private void Chase(){
        Vector2 direction = (player.position - transform.position).normalized;
        if(rig.velocity.y == 0)
            rig.velocity = new Vector2(direction.x * moveSpeed, enemy.rig.velocity.y);  
  
    }
}
