using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]

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
    [Header("Sound Effects")]
    [Header("Attack Sounds")]
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    [Header("Combat Misc Voices")]
    public AudioClip killed;
    public AudioClip teleported;

    private PlayerAnimController spriteAnimator;
    private UIBridge bridge;
    private AudioSource audioSource;
    private bool hasDoneAttackSound = false;
    private bool isFrozen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        bridge = GameObject.Find("Canvas").GetComponent<UIBridge>();
        spriteAnimator = transform.Find("PlayerSprite").GetComponent<PlayerAnimController>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        bridge.DisplayText("", 2.0f);
    }

    IEnumerator FreezeAsPartOfLore()
    {
        float timer = 20f;
        float audioOffset = 0f;
        audioSource.PlayOneShot(teleported);
        isFrozen = true;
        agent.isStopped = true;
        spriteAnimator.SetWalkBoolean(false);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            audioOffset += Time.deltaTime;
            if (audioOffset >= 0.105f && audioOffset < 0.105f + Time.deltaTime) bridge.DisplayText("Wha.. wha.. what.. *gasps*. Why??", 3.286f);
            else if (audioOffset >= 3.939f && audioOffset < 3.939f + Time.deltaTime) bridge.DisplayText("Why am I teleported back here?", 2.246f);
            else if (audioOffset >= 8.189f && audioOffset < 8.189f + Time.deltaTime) bridge.DisplayText("Didn't that just... that big goblin...", 3.213f);
            else if (audioOffset >= 12.680f && audioOffset < 12.680f + Time.deltaTime) bridge.DisplayText("...granted me superpowers, but....", 1.693f);
            else if (audioOffset >= 16.412f && audioOffset < 16.412f + Time.deltaTime) bridge.DisplayText("...it made me right back here? How ridiculous!", 4.008f);

            yield return null;
        } 
        isFrozen = false;
        agent.isStopped = false;
        agent.SetDestination(new Vector3(0, -1.5f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFrozen)
        {
            var mouse = Mouse.current;
            if (mouse == null || Camera.main == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                bridge.DisplayText("*thuds*", 1.0f);
                Debug.Log("Attacking");
                Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y - 1.5f), attackRadius);
                Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - 1.5f) + Vector2.right * 0.1f, new Vector2(transform.position.x, transform.position.y - 1.5f) - Vector2.right * 0.1f, Color.red, attackRadius);
                foreach (var hit in hits)
                {
                    if (hit.GetComponent<DamageableBug>() != null)
                    {
                        float damageDealt = damage * damageMultiplier;
                        Debug.Log("Dealt " + damageDealt + " to " + hit.name);
                        hit.GetComponent<DamageableBug>().TakeDamage(damageDealt);
                        if (hit.GetComponent<DamageableBug>().GetMeHealth() <= 0)
                        {
                            if (hit.GetComponent<DamageableBug>().WillBeTeleported())
                            {
                                StartCoroutine(FreezeAsPartOfLore());
                            }
                            else
                            {
                                audioSource.PlayOneShot(killed);
                                bridge.DisplayText("Alright, one down! That's for me!", 3.549f);
                            }
                        }
                        else if (hit.GetComponent<DamageableBug>().GetMeHealth() <= 25)
                        {
                            // Enemy is below or equal to 10 health, don't play attack sound
                        }
                        else
                        {
                            if (!hasDoneAttackSound)
                            {
                                audioSource.PlayOneShot(attackSound1);
                                hasDoneAttackSound = true;
                            }
                            else
                            {
                                audioSource.PlayOneShot(attackSound2);
                                hasDoneAttackSound = false;
                            }
                        }
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
