using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BotAttack : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 0.75f;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;

    public int attackDamage = 20; // Damage inflicted by the bot

    private Animator animator;
    private Transform player;
    private PlayerHealth playerHealth;
    private bool isFacingRight = true;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Find the player by tag and assign components
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null)
            return; // Exit if player is not assigned

        attackTimer -= Time.deltaTime;

        if (!isAttacking)
        {
            MoveAndAttackPlayer();
        }
    }

    void MoveAndAttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is alive and within attack range, and attack cooldown is ready
        if (playerHealth != null && playerHealth.IsAlive && distanceToPlayer <= attackRange && attackTimer <= 0)
        {
            Attack();
        }
        else if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
    }


    void MoveTowardsPlayer()
    {
        // Move towards player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Check and flip direction if needed
        if ((direction.x > 0 && isFacingRight) || (direction.x < 0 && !isFacingRight))
        {
            Flip();
        }
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Reset attack timer
        attackTimer = attackCooldown;
        StartCoroutine(EndAttackAfterAnimation());
    }

    IEnumerator EndAttackAfterAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        isAttacking = false;
    }

    void ApplyDamage()
    {
        // Apply damage if within range
        if (playerHealth != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
