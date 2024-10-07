using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Goblin : Enemy
{
    private float moveSpeed = 4f; // Velocidade de movimento do inimigo
    private float bombRange = 10f;
    private GoblinBombShot bombThrow;
    private float timer = 5f;
    private AudioSource goblinAudioSource;
    public AudioClip goblinLaugh;
    private System.Random random;
    private int selectedAction = -1;

    private void Start(){
        currentHealth = maxHealth; 
        order = "Demi-human";
        species = "Goblin";
        enemy = GetComponent<Enemy>();
        bombThrow = GetComponent<GoblinBombShot>();
        goblinAudioSource = GetComponent<AudioSource>();
        random = new System.Random();
        canAttack = true;
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }
    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            StartCoroutine(Attack());
        }

        if (distanceToPlayer < detectionRange && distanceToPlayer < bombRange)
        {

            timer += Time.deltaTime;

            if (selectedAction == -1) // Only calculate probability when action is not set
            {
                int bombthrowProbability = random.Next(0, 100);
                if (bombthrowProbability < 40)
                {
                    selectedAction = 0; // Set action for the first probability range
                }
                else if (bombthrowProbability < 80)
                {
                    selectedAction = 1; // Set action for the second probability range
                }
                else
                {
                    selectedAction = 2; // Set action for the third probability range
                }
            }

            // Execute the selected action based on the timer
            if (selectedAction == 0)
            {
                if (timer > 5f)
                {
                    timer = 0f;
                    StartCoroutine(HandleBombThrow());
                    selectedAction = -1; // Reset action after execution
                }
            }
            else if (selectedAction == 1)
            {
                if (timer > 2f)
                {
                    timer = 0f;
                    StartCoroutine(HandleBombThrow());
                    selectedAction = -1; // Reset action after execution
                }
            }
            else
            {
                if (timer > 5f)
                {
                    Debug.Log("goblin parou");
                    timer = 0f;
                    selectedAction = -1; // Reset action even if no action is taken
                }
            }
        }
        else
        {
            // Reset everything if the player is out of range
            selectedAction = -1;
            timer = 0f;
        }

        if (distanceToPlayer < detectionRange && distanceToPlayer > attackRange)
        {
            Chase();   
        }
    }

    private IEnumerator HandleBombThrow()
    {
        float originalMoveSpeed = moveSpeed;
        moveSpeed = 0;
        
        goblinAudioSource.PlayOneShot(goblinLaugh);
        // Start bomb throw animation
        animator.SetTrigger("bombThrow");

        // Wait for the animation to finish
        yield return new WaitForSeconds(0.75f);

        // Throw the bomb after the animation completes
        bombThrow.ThrowBomb();

        // Short delay after throwing the bomb (if necessary)
        yield return new WaitForSeconds(0.3f);

        // Restore the movement speed
        moveSpeed = originalMoveSpeed;
    }

    private float GetAnimationLength(string animationName)
    {
        // Get the AnimatorStateInfo for the current animation state
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);

        // Verify if the current animation matches the one we want
        if (animationState.IsName(animationName))
        {
            return animationState.length;
        }

        // If not, return a default value (or handle as necessary)
        return 0f;
    }    public override IEnumerator Attack(){
        if(canAttack){
            // plays attack animation
            canAttack = false;
            animator.SetTrigger("attack1");
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
        }
    }
    private void Chase(){
        Vector2 direction = (player.position - transform.position).normalized;
        if(rig.velocity.y == 0)
            rig.velocity = new Vector2(direction.x * moveSpeed, enemy.rig.velocity.y);  
  
    }
}
