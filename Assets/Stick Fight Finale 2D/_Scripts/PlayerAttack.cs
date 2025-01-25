using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public KeyCode punchAttackKey;
    public KeyCode kickAttackKey;
    public KeyCode specialAttackKey;

    public float jumpForce = 4f;

    public float punchAttackCooldown = 1f;
    public float kickAttackCooldown = 1f;
    public float specialAttackCooldown = 5f;

    private float punchTimer = 0f;
    private float kickTimer = 0f;
    private float specialAttackTimer = 0f;

    public Image punchAttackCooldownImage;
    public Image kickAttackCooldownImage;
    public Image specialAttackCooldownImage;

    public int punchAttackDamage = 20;
    public int kickAttackDamage = 30;
    public int specialAttackDamage = 100;

    private Animator animator;

    public AudioSource punchAttackAudioSource;
    public AudioClip punchAttackSound;
    public AudioSource kickAttackAudioSource;
    public AudioClip kickAttackSound;
    public AudioSource specialAttackAudioSource;
    public AudioClip specialAttackSound;

    private PlayerMovement playerMovement;

    public float attackRange = 1f;
    public bool isFacingRight = true;

    private bool isAttacking = false; // Flag to track if the player is in an attack animation

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        punchAttackCooldownImage.fillAmount = 1;
        kickAttackCooldownImage.fillAmount = 1;
        specialAttackCooldownImage.fillAmount = 1;
        playerMovement = GetComponent<PlayerMovement>();

        // Initialize audio sources if they are null
        if (punchAttackAudioSource == null) punchAttackAudioSource = gameObject.AddComponent<AudioSource>();
        punchAttackAudioSource.clip = punchAttackSound;
        punchAttackAudioSource.loop = false;

        if (kickAttackAudioSource == null) kickAttackAudioSource = gameObject.AddComponent<AudioSource>();
        kickAttackAudioSource.clip = kickAttackSound;
        kickAttackAudioSource.loop = false;

        if (specialAttackAudioSource == null) specialAttackAudioSource = gameObject.AddComponent<AudioSource>();
        specialAttackAudioSource.clip = specialAttackSound;
        specialAttackAudioSource.loop = false;
    }

    void Update()
    {
        punchTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;
        specialAttackTimer -= Time.deltaTime;

        // Check direction
        if (playerMovement != null)
        {
            isFacingRight = playerMovement.isFacingRight;
        }

        if (Input.GetKeyDown(punchAttackKey) && punchTimer <= 0 && !isAttacking)
        {
            StartAttack("Punch", punchAttackDamage, punchAttackCooldown, punchAttackAudioSource, punchAttackSound);
        }

        if (Input.GetKeyDown(kickAttackKey) && kickTimer <= 0 && !isAttacking)
        {
            StartAttack("Kick", kickAttackDamage, kickAttackCooldown, kickAttackAudioSource, kickAttackSound);
        }

        if (Input.GetKeyDown(specialAttackKey) && specialAttackTimer <= 0 && !isAttacking)
        {
            StartAttack("Special", specialAttackDamage, specialAttackCooldown, specialAttackAudioSource, specialAttackSound);
        }

        UpdateCooldownUI();
    }

    void StartAttack(string attackType, int damage, float cooldown, AudioSource audioSource, AudioClip sound)
    {
        isAttacking = true; // Mark the player as attacking
        animator.SetTrigger(attackType);
        audioSource.PlayOneShot(sound);

        if (attackType == "Special")
        {
            // Add vertical force for the jump effect
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        Vector2 attackPosition = transform.position + Vector3.right * (isFacingRight ? attackRange : -attackRange);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            BotHealth botHealth = enemy.GetComponent<BotHealth>();
            if (botHealth != null)
            {
                botHealth.TakeDamage(damage);
                break; // Only hit one bot per attack
            }
        }

        if (attackType == "Punch")
        {
            punchTimer = cooldown;
        }
        else if (attackType == "Kick")
        {
            kickTimer = cooldown;
        }
        else if (attackType == "Special")
        {
            specialAttackTimer = cooldown;
        }

        // Re-enable movement after a short duration (matching attack animation length)
        Invoke(nameof(EndAttack), 0.1f); // Adjust this duration based on the animation length
    }


    void EndAttack()
    {
        isAttacking = false; // Allow movement and other actions again
    }

    void UpdateCooldownUI()
    {
        punchAttackCooldownImage.fillAmount = Mathf.Clamp(punchTimer / punchAttackCooldown, 0, 1);
        if (punchTimer <= 0)
        {
            punchAttackCooldownImage.fillAmount = 1;
        }

        kickAttackCooldownImage.fillAmount = Mathf.Clamp(kickTimer / kickAttackCooldown, 0, 1);
        if (kickTimer <= 0)
        {
            kickAttackCooldownImage.fillAmount = 1;
        }

        specialAttackCooldownImage.fillAmount = Mathf.Clamp(specialAttackTimer / specialAttackCooldown, 0, 1);
        if (specialAttackTimer <= 0)
        {
            specialAttackCooldownImage.fillAmount = 1;
        }
    }
}
