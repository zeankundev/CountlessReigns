using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DamageableBug : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float keepDistance = 1.2f; // Distance to stop before hitting player

    [Header("Bug Stats")]
    public float damage = 20;
    public float health = 300;
    public float maxHealth = 300;

    [Header("Post-Kill Rewards")]
    public float xp = 75;
    public float damageMultiplier = 1.5f;
    public float extraHealth = 25;
    public float extraMaxHealth = 100;

    [Header("Consequences")]
    public bool teleportPlayerBack = false;
    public Vector2 teleportLocation = new Vector2(0, -1.5f);
    
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Animator animator;
    private RectTransform healthBar;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // 2D Physics/Rotation setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Set the stopping distance so it doesn't push the player
        agent.stoppingDistance = keepDistance;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        healthBar = transform.Find("HealthBar").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // The agent will now automatically stop 'keepDistance' units away
            agent.SetDestination(playerTransform.position);
            animator.SetBool("Is Moving", true);
        }
        else
        {
            // Clear path if player escapes radius
            if (agent.hasPath) agent.ResetPath();
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            animator.SetBool("Is Moving", false);
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Bug took " + amount + " damage.");
        health -= amount;
        healthBar.localScale = new Vector3(Mathf.Clamp01(health / maxHealth * 1.849104f), 0.1146779f, 1);
        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        PlayerController player = playerTransform.GetComponent<PlayerController>();
        if (player != null)
        {
            player.GrantPlayerSomeBuff(damageMultiplier, extraHealth, extraMaxHealth, xp);
        }
        if (teleportPlayerBack && playerTransform != null)
        {
            playerTransform.position = new Vector3(teleportLocation.x, teleportLocation.y, playerTransform.position.z);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualize stop distance (the "no-push" zone)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, keepDistance);
    }
}