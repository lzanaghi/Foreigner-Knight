using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class GoblinBomb : MonoBehaviour
{
    private GameObject player;
    private Goblin goblin;
    private Rigidbody2D rb;
    private Collider2D bombHitbox;
    public Collider2D explosionHitbox;
    private System.Random random;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip bombExplodeSFX;
    public AudioClip bombFuseSFX;
    public AudioSource bombThrowSFX;

    private int bombDamage;
    private float throwForce;
    private bool hasExploded = false;

    public void SetGoblinReference(Goblin goblinRef)
    {
        goblin = goblinRef;
    }

    void Start()
    {
        random = new System.Random();
        rb = GetComponent<Rigidbody2D>();
        bombHitbox = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (goblin == null)
        {
            Debug.LogError("Goblin component not found.");
            return;
        }

        bombDamage = goblin.attackDamage * 2;
        explosionHitbox.enabled = false;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing on this GameObject.");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        int randomThrowForce = random.Next(15, 30);

        bombThrowSFX.Play(); //play bomb sfx
        audioSource.PlayOneShot(bombFuseSFX);
        throwForce = 3f;
        Vector2 throwDistance;
        if (goblin.isFacingRight)
            throwDistance = new Vector2(randomThrowForce * 0.1F * throwForce, -transform.position.y * throwForce*1.4f);    
        else
            throwDistance = new Vector2(-randomThrowForce * 0.1F * throwForce, -transform.position.y * throwForce*1.4f);
        rb.AddForce(throwDistance, ForceMode2D.Impulse);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(hasExploded) return;
        // Verify if the bomb hit the player
        if (bombHitbox != null && collision.gameObject.CompareTag("Player"))
        {
            PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();
            if (player != null)
            {
                audioSource.Stop();
                StartCoroutine(DetonateBomb()); // bomb explodes
            }
        }else if(bombHitbox != null && collision.gameObject.layer == 8){
            StartCoroutine(DelayedExplosion());
        }
    }

    private IEnumerator DetonateBomb(){
        bombHitbox.enabled = false;
        bombThrowSFX.Stop();
        rb.velocity = Vector2.zero; //stop bomb movement
        rb.isKinematic = true; // deisable bomb physics
        animator.SetTrigger("explode");
        audioSource.PlayOneShot(bombExplodeSFX);
        hasExploded = true;
        //explosionHitbox.enabled = true;
        yield return new WaitForSeconds(0.8f);
        //explosionHitbox.enabled = false;
        Destroy(gameObject);
   }
   void OnTriggerEnter2D(Collider2D other)
    {
        if (explosionHitbox.enabled && other.CompareTag("Player"))
        {
            // Acessa o script do jogador e aplica dano
            PlayerCombat player = other.GetComponent<PlayerCombat>();
            if (player != null)
            {
                player.TakeDamage(bombDamage);
            }
        }
    }
    IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(3f);
        if (!hasExploded)
        {
            audioSource.Stop();
            animator.SetTrigger("detonate");
            yield return new WaitForSeconds(0.6f);
            StartCoroutine(DetonateBomb());
        }
    }

    void ActivateExplosionHitbox(){
        explosionHitbox.enabled = true;
    }

    void DeactivateExplosionHitbox(){
        explosionHitbox.enabled = false;
    }

}