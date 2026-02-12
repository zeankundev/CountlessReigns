using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DamageableBug : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float keepDistance = 1.2f; // Distance to stop before hitting player
    public float attackRadius = 1.0f;

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
    private TMP_Text damageText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Transform damageTextTransform = transform.Find("DamageText");
        if (damageTextTransform != null)
        {
            Debug.Log("DamageText not null");
            damageText = damageTextTransform.GetComponent<TMP_Text>();
            Debug.Log("DamageText component: " + damageText);
        }
        
        // 2D Physics/Rotation setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Set the stopping distance so it doesn't push the player
        agent.stoppingDistance = keepDistance;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        healthBar = transform.Find("HealthBar/Display").GetComponent<RectTransform>();
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
        if (distanceToPlayer <= attackRadius)
        {
            // Attack the player
            animator.SetTrigger("Attack");
            playerTransform.GetComponent<PlayerController>().TakeDamage(damage);
            agent.ResetPath(); // Stop moving while attacking
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
        healthBar.sizeDelta = new Vector2((health / maxHealth) * 2, healthBar.sizeDelta.y);
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(ShowDamageText(amount));
        animator.SetTrigger("Take Damage");
    }
    public float GetMeHealth()
    {
        return health;
    }
    public float GetMeMaxHealth()
    {
        return maxHealth;
    }
    public bool WillBeTeleported()
    {
        return teleportPlayerBack;
    }
    IEnumerator ShowDamageText(float amount)
    {
        if (damageText != null)
        {
            Debug.Log("Showing damage text: " + amount);
            damageText.text = amount.ToString();
            damageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            damageText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Damage text component is null.");
            yield return null;
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