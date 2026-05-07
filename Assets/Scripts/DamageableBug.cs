using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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
    public bool allowUseOfPoisonSyringe = false;

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

    private bool canAttack = true; // Add this field

    IEnumerator AttackCooldown()
    {
        canAttack = false; // Block attacks immediately
        float cooldown = 1.0f;

        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime; // Count down every second in real time
            yield return null;         // Wait one frame, then loop
        }

        canAttack = true; // Re-enable attacking after cooldown
    }

    IEnumerator PoisonSyringeEffect()
    {
        float duration = 5f;
        float timer = 0f;
        while (timer < duration)
        {
            TakeDamage(5 * Mathf.Pow(2, 5 - (health / maxHealth) * 5) * Time.deltaTime); // Apply poison damage over time
            timer += Time.deltaTime;
            yield return new WaitForSeconds(0.5f); // Apply damage every 0.5 seconds
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(playerTransform.position);
            animator.SetBool("Is Moving", true);
            bool playerHasPoisonSyringe = playerTransform.GetComponent<PlayerController>().inventory.Contains("Poison Syringe");
            if (health / maxHealth <= 0.15f)
            {
                if (Keyboard.current.eKey.wasPressedThisFrame && allowUseOfPoisonSyringe && playerHasPoisonSyringe)
                {
                    Debug.Log("Using poison syringe on bug!");
                    // Take an exponential amount of damage on one shot. First, it will deal a small damage, but as over time, the damage will increase dramatically, encouraging the player to use it early on and not wait until the bug is almost dead.
                    StartCoroutine(PoisonSyringeEffect());
                }
            }
        }
        else
        {
            if (agent.hasPath) agent.ResetPath();
        }

        if (distanceToPlayer <= attackRadius && canAttack) // ← Guard with canAttack
        {
            animator.SetTrigger("Attack");
            playerTransform.GetComponent<PlayerController>().TakeDamage(damage);
            StartCoroutine(AttackCooldown());
            agent.ResetPath();
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}