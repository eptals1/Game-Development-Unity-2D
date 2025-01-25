using UnityEngine;

public class AidKit : MonoBehaviour
{
    public int healAmount = 20; // Amount of health to restore

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject); // Destroy the aid kit after use
            }
        }
    }
}
