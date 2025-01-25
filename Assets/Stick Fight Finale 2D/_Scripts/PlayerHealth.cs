using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBar;
    private Animator animator;

    public AudioSource hitAudioSource;
    public AudioClip hitAudioClip;

    public delegate void PlayerDeathHandler();
    public static event PlayerDeathHandler OnPlayerDeath;

    public PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private GameOverManager gameOverManager; // Reference to GameOverManager script

    public bool IsAlive => currentHealth > 0;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        gameOverManager = FindAnyObjectByType<GameOverManager>(); // Find any GameOverManager in the scene
        UpdateHealthBar();

        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");
        if (hitAudioSource != null && hitAudioClip != null)
        {
            hitAudioSource.PlayOneShot(hitAudioClip);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
    }

    // Movement will be disabled once the animation event triggers OnDeathAnimationEnd
    // This method is called by the animation event at the end of the death animation
    void OnDeathAnimationEnd()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;

            if (playerMovement.movementAudioSource != null)
            {
                playerMovement.movementAudioSource.Stop();
            }

            playerMovement = null;
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = false;
            playerAttack = null;
        }

        Time.timeScale = 0f;
        OnPlayerDeath?.Invoke();
        gameOverManager.ShowGameOverUI(); // Show Game Over UI
    }
}
