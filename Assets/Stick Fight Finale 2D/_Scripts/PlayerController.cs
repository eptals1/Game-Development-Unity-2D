using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.C;
    public KeyCode punchAttackKey = KeyCode.F;
    public KeyCode kickAttackKey = KeyCode.G;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = true;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool isAttacking = false;

    private float dashTimer = 0f;
    private float punchTimer = 0f;
    private float kickTimer = 0f;

    public float punchAttackCooldown = 1f;
    public float kickAttackCooldown = 1f;

    public Image dashCooldownImage;
    public Image punchAttackCooldownImage;
    public Image kickAttackCooldownImage;

    public AudioSource movementAudioSource;
    public AudioSource dashAudioSource;
    public AudioSource punchAttackAudioSource;
    public AudioSource kickAttackAudioSource;

    public AudioClip runSound;
    public AudioClip dashSound;
    public AudioClip punchAttackSound;
    public AudioClip kickAttackSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InitializeAudioSource(movementAudioSource, runSound, true);
        InitializeAudioSource(dashAudioSource, dashSound, false);
        InitializeAudioSource(punchAttackAudioSource, punchAttackSound, false);
        InitializeAudioSource(kickAttackAudioSource, kickAttackSound, false);

        UpdateCooldownUI();
    }

    void Update()
    {
        dashTimer -= Time.deltaTime;
        punchTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        if (!isDashing && !isAttacking)
        {
            MovePlayer();
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                Jump();
            }
        }

        if (Input.GetKeyDown(dashKey) && dashTimer <= 0 && isGrounded)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(punchAttackKey) && punchTimer <= 0)
        {
            StartCoroutine(PerformAttack("Punch", punchAttackCooldown, punchAttackAudioSource));
        }

        if (Input.GetKeyDown(kickAttackKey) && kickTimer <= 0)
        {
            StartCoroutine(PerformAttack("Kick", kickAttackCooldown, kickAttackAudioSource));
        }

        UpdateCooldownUI();
        AnimatePlayer();
    }

    void MovePlayer()
    {
        float moveInput = Input.GetKey(leftKey) ? -moveSpeed : (Input.GetKey(rightKey) ? moveSpeed : 0f);
        if (moveInput != 0 && (moveInput > 0 != isFacingRight))
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(moveInput, rb.linearVelocity.y);
        PlayRunningSound(moveInput);
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetTrigger("Dash");
        dashAudioSource.Play();

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(isFacingRight ? dashSpeed : -dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
        dashTimer = dashCooldown;
    }

    IEnumerator PerformAttack(string attackTrigger, float cooldown, AudioSource audioSource)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);
        audioSource.Play();

        yield return new WaitForSeconds(cooldown);

        isAttacking = false;
    }

    void AnimatePlayer()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", !isGrounded);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void PlayRunningSound(float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.1f && isGrounded && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
        else if (Mathf.Abs(moveInput) <= 0.1f || !isGrounded)
        {
            movementAudioSource.Stop();
        }
    }

    void InitializeAudioSource(AudioSource audioSource, AudioClip clip, bool loop)
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clip;
        audioSource.loop = loop;
    }

    void UpdateCooldownUI()
    {
        dashCooldownImage.fillAmount = Mathf.Clamp01(dashTimer / dashCooldown);
        punchAttackCooldownImage.fillAmount = Mathf.Clamp01(punchTimer / punchAttackCooldown);
        kickAttackCooldownImage.fillAmount = Mathf.Clamp01(kickTimer / kickAttackCooldown);
    }
}
