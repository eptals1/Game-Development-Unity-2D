using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 3f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.C;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = true;
    public bool isFacingRight = true;
    private bool isDashing = false;
    public AudioSource movementAudioSource;
    public AudioClip runSound;

    public AudioSource dashAudioSource;
    public AudioClip dashSound;

    public float dashCooldown = 3f;
    private float dashTimer = 0f;
    public Image dashCooldownImage;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashCooldownImage.fillAmount = 1;

        if (movementAudioSource == null)
        {
            movementAudioSource = gameObject.AddComponent<AudioSource>();
        }
        movementAudioSource.clip = runSound;
        movementAudioSource.loop = true;

        if (dashAudioSource == null)
        {
            dashAudioSource = gameObject.AddComponent<AudioSource>();
        }
        dashAudioSource.clip = dashSound;
        dashAudioSource.loop = false;
    }

    void Update()
    {
        dashTimer -= Time.deltaTime;

        if (!isDashing)
        {
            MovePlayer();
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                Jump();
            }
        }

        if (Input.GetKeyDown(dashKey) && !isDashing && isGrounded && dashTimer <= 0)
        {
            StartCoroutine(Dash());
        }

        AnimatePlayer();
        UpdateCooldownUI();
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
        animator.SetTrigger("Jump");
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

    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetTrigger("Dash");
        dashAudioSource.Play();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bot"), true);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(isFacingRight ? dashSpeed : -dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bot"), false);
        isDashing = false;
        dashTimer = dashCooldown;
        UpdateCooldownUI();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void UpdateCooldownUI()
    {
        dashCooldownImage.fillAmount = Mathf.Clamp(dashTimer / dashCooldown, 0, 1);
        if (dashTimer <= 0)
        {
            dashCooldownImage.fillAmount = 1;
        }
    }

    void PlayRunningSound(float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.1f && isGrounded)
        {
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();
            }
        }
        else
        {
            movementAudioSource.Stop();
        }
    }
}
