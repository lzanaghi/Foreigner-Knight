using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : Player
{

    public float Speed;
    private float horizontal;
    public float dash_force;
    public bool isGrounded;

    //object references
    private Rigidbody2D rig;
    private Animator animator;
    [SerializeField]private TrailRenderer tr;
    public GhostingEffect ghost;
    public PlayerCombat playerCombat;

    [Header("SFX")]
    public AudioSource runSFX;
    public AudioSource jumpSFX;
    public AudioSource jumpFallSFX;
    public AudioSource rollSFX;
    public AudioSource wallJumpSFX;
    public float runSoundInterval = 0.01f; // Intervalo de tempo entre as verificações do som
    private float nextRunSoundTime = 0f;  // Tempo para a próxima verificação
    float timer = 0f;

    [Header("Horizontal Movement")]
    public bool isFacingRight = true;
    private bool canMove = true;

    [Header("Jump")]
    private bool isJumping, isFalling;
    public bool canJump = true;
    public float jumpStartTime;
    public float jump_force;
    private float jumpTime;

    //dash
    private bool canDash = true;
    protected bool isDashing;
    private float dashingPower = 15f;
    private float dashingTime = 0.15f;
    private float dashingCooldown = 1f;

    [Header("Roll")]
    private bool isRolling;
    public bool canRoll = true;
    private float rollCooldown = 0.2f;
    private float actionRollCooldown = 0.6f;

    [Header("Wall Slide")]
    private bool isWallSliding;
    private float wallSlidingSpeed = 1.8f;
    private bool isWalled;

    [Header("Wall Jump")]
    private bool isJumpingFromWall;
    private float wallJumpingFinish;
    private float wallJumpingDuration = 0.4f;
    private float wallJumpingPower = 12f;

    [SerializeField]private Transform wallCheck;
    [SerializeField]private LayerMask wallLayer;
   
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update(){
        WallJump();
        if(isJumpingFromWall){
            if(Time.time > wallJumpingFinish){
                isJumpingFromWall = false;
            }
        }

        if(!isJumpingFromWall && !canMove){
            if(Input.GetAxis("Horizontal") != 0 || isGrounded){
                canMove = true;
            }
        }


        if(!isGrounded){
            ghost.makeGhosting = true;
            return;
        }else{
            ghost.makeGhosting = false;
            if (Input.GetButtonDown("Jump") && isGrounded && canJump)
            {
                StartCoroutine(Jump());
            }
        }
        if(isRolling){
            ghost.makeGhosting = true;
            return;
        }else{
            if(Input.GetKeyDown(KeyCode.X) && canRoll && !isJumping){
                StartCoroutine(Roll());
            }

        }
    }

    void FixedUpdate()
    {
        Move();
        Fall();
        WallSlide();
        //Crouch();

        if(isDashing){
            return;
        }else{
            if(Input.GetKeyDown(KeyCode.C) && canDash && !isJumping){
                StartCoroutine(Dash());
            }
        }
    }

    void Move()
    {
        if (isJumpingFromWall)
        {
            // Durante o wall jump, não modifique a velocidade horizontal
            return;
        }
        if(canMove){
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
            horizontal = movement.x;

            //avoid weird wall colision
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * movement, 0.1f);

            rig.velocity = new Vector2(movement.x * Speed, rig.velocity.y);
            timer += Time.deltaTime;
            if (Input.GetAxis("Horizontal") > 0f)
            {
                if (timer > 0.2f){
                    PlayRunningAudio();
                    timer = 0;
                }
                animator.SetBool("run", true);

                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                isFacingRight = true;
            }
            else if (Input.GetAxis("Horizontal") < 0f)
            {
                if (timer > 0.2f){
                    PlayRunningAudio();
                    timer = 0;
                }
                animator.SetBool("run", true);
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                isFacingRight = false; 
            }
            else
            {
                animator.SetBool("run", false);
            }
        }
    }

    IEnumerator Jump()
    {
        if (isJumpingFromWall)
        {
            // Durante o wall jump, não modifique a velocidade horizontal
            yield break;
        }
        isJumping = true;
        canRoll = false;
        jumpTime = jumpStartTime;

        // Aplica uma força inicial mais forte para o início do pulo
        rig.velocity = Vector2.up * jump_force * 0.2f;
        jumpSFX.Play();
        animator.SetBool("jump", true);

        // Enquanto o botão de pulo estiver pressionado e ainda houver tempo de pulo
        while (Input.GetButton("Jump") && jumpTime > 0)
        {
            // Continua aplicando uma força de pulo, talvez um pouco menor que a inicial para suavizar o movimento
            rig.velocity = new Vector2(rig.velocity.x, jump_force * 0.75f);
            jumpTime -= Time.deltaTime;
            yield return null; // Espera até o próximo frame
        }

        // Espera até o personagem aterrissar novamente
        yield return new WaitUntil(() => isGrounded);
        isJumping = false;
        canRoll = true;
        jumpFallSFX.Play();
        animator.SetBool("jump", false);
    }

    void Fall()
    {
        isPlayerFalling();
        if (isFalling)
        {
            animator.SetBool("jumpfall", true);
        }
        else
        {
            animator.SetBool("jumpfall", false);
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        canJump = false;
        isDashing = true;
        float originalGravity = rig.gravityScale;
        rig.gravityScale = 0f;
        if(isFacingRight)
        {
            rig.AddForce(new Vector2(dashingPower, 0f), ForceMode2D.Impulse);    
        }
        else
        {
            rig.AddForce(new Vector2(-dashingPower, 0f), ForceMode2D.Impulse);    
        }
        animator.SetBool("slide", true);

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        rig.velocity = new Vector3(0f,0f,0f);
        canJump = true;
        tr.emitting = false;
        rig.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool("slide", false);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;  
        
    }

    IEnumerator Roll(){
        if (isJumping)
            yield break;
        canAttack = false;
        canJump = false;
        canRoll = false;
        StartCoroutine(PlayRollSFX());
        animator.SetTrigger("player_roll");
        gameObject.layer = 12;
        ActivateRoll();
        yield return new WaitForSeconds(actionRollCooldown);
        canAttack = true;
        canJump = true;
        ghost.makeGhosting = false;
        yield return new WaitForSeconds(rollCooldown);
        DeactivateRoll();
        gameObject.layer = 3;
        canRoll = true;
    }

    void Crouch(){
        Vector3 crouch = new Vector3(0f, Input.GetAxis("Vertical"), 0f);
        if (Input.GetAxis("Vertical") < 0f){
            animator.SetBool("player_crouch", true);
        }else{
            animator.SetBool("player_crouch", false);
            animator.SetTrigger("player_getup");
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isGrounded = true;
        }
        if (collision.gameObject.layer == 15)
        {
            isWalled = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            isGrounded = false;
        }
        if (collision.gameObject.layer == 15)
        {
            isWalled = false;
        }
    } 

  

    private void WallSlide(){
        if(isWalled && !isGrounded && isFalling && Math.Abs(horizontal) > 0f){
            animator.SetBool("player_wall_slide", true);
            isWallSliding = true;
            rig.velocity = new Vector2(rig.velocity.x, Mathf.Clamp(rig.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }else{
            animator.SetBool("player_wall_slide", false);
            isWallSliding = false;
        }
    }

    private void WallJump(){
        if(Input.GetButtonDown("Jump") && isWallSliding){
            canMove = false;
            wallJumpSFX.Play();
            animator.SetTrigger("jump");
            wallJumpingFinish = Time.time + wallJumpingDuration;
            isJumpingFromWall = true;

            rig.velocity = Vector2.zero;
            
            int direction;
            if(isFacingRight){
                direction = -1;
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }else{
                direction = 1;
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            isFacingRight = !isFacingRight;
            rig.AddForce(new Vector2(direction * 0.6f * wallJumpingPower, wallJumpingPower), ForceMode2D.Impulse);   
        }
    }

    void Flip()
    {
        // Inverte a direção do jogador
        isFacingRight = !isFacingRight;

        // Inverte a escala do jogador no eixo x
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void isPlayerFalling()
    {
        if(rig.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    private void PlayRunningAudio()
    {
 
        // Atualiza o tempo para a próxima verificação
        nextRunSoundTime = Time.time + runSoundInterval;

        // Verifica se o jogador está na animação de corrida, não está rolando e está no chão
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_running") && !isRolling && isGrounded)
        {
            runSFX.Play();
        }
        else
        {
            if (runSFX.isPlaying)
            {
                StopRunningAudio();    
            }
        }

    }
    private void StopRunningAudio(){
        runSFX.Stop();
    }
    private IEnumerator PlayRollSFX(){
        yield return new WaitForSeconds(0.4f);
        rollSFX.Play();
    }

    void ActivateRoll(){
        isRolling = true;
        playerCombat.canTakeDamage = false;  
    }

    void DeactivateRoll(){
        isRolling = false;
    }

    void ActivateRollInvincibility(){
        playerCombat.canTakeDamage = false;       
    }

    void DeactivateRollInvincibility(){
        playerCombat.canTakeDamage = true;
    }

}
