using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject idk;

    [Header("Player Stats")]
    public float health = 100;
    public float maxHealth = 100;
    public float damage = 25;
    public float damageMultiplier = 1.0f;
    public float xp = 0;
    public float attackRadius = 1f;
    private PlayerAnimController spriteAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteAnimator = transform.Find("PlayerSprite").GetComponent<PlayerAnimController>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || Camera.main == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Attacking");
            Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y - 1.5f), attackRadius);
            Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - 1.5f) + Vector2.right * 0.1f, new Vector2(transform.position.x, transform.position.y - 1.5f) - Vector2.right * 0.1f, Color.red, attackRadius);
            foreach (var hit in hits)
            {
                if (hit.GetComponent<DamageableBug>() != null)
                {
                    hit.GetComponent<DamageableBug>().TakeDamage(damage * damageMultiplier);
                    break;
                }
            }
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Right click detected");
            Vector2 mousePos = mouse.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // We "fire" a raycast at a single point (direction zero, distance zero)
            // to see if there is a 2D collider underneath the cursor.
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name);
                
                GameObject thingy = Instantiate(idk, new Vector3(hit.point.x, hit.point.y, 0), Quaternion.identity);
                // Now hit.point works because 'hit' is a RaycastHit2D!
                agent.SetDestination(hit.point);
                spriteAnimator.SetWalkBoolean(true);
                Destroy(thingy, 0.2f);
            }
            else
            {
                Debug.Log("No collider hit");
            }
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            spriteAnimator.SetWalkBoolean(false);
        }
    }
    public void HealPlayer(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
    }
    public void GrantPlayerSomeBuff(float extraDamageMultiplier, float extraHealthAmount, float extraMaxHealthAmount, float extraXP)
    {
        damageMultiplier += extraDamageMultiplier;
        maxHealth += extraMaxHealthAmount;
        health += extraHealthAmount;
        if (health > maxHealth) health = maxHealth;
        xp += extraXP;
    }
}
