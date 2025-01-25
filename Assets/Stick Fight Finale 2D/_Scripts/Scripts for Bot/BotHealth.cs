using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BotHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Slider healthBar; // Reference to the bot's health bar slider UI

    public BotManager botManager; // Reference to manage the bot's lifecycle

    private Animator animator;
    private bool isDying = false; // Flag to prevent redundant death logic

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();


    }

    public void TakeDamage(int damage)
    {
        if (isDying) return; // Prevent damage during death

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        animator.SetTrigger("Hit");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            // Notify the BotManager about the bot's death
            if (botManager != null)
            {
                botManager.OnBotKilled();
            }
            animator.SetTrigger("Die");

            OnDeathAnimationComplete();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }


    // This method can be invoked from the death animation event
    void OnDeathAnimationComplete()
    {
        Destroy(gameObject);

    }
}
