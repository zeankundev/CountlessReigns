using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DamageableBug : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float keepDistance = 1.2f; // Distance to stop before hitting player

    [Header("Bug Stats")]
    public float damage = 20;
    public float health = 100;
    
    private NavMeshAgent agent;
    private Transform playerTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // 2D Physics/Rotation setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Set the stopping distance so it doesn't push the player
        agent.stoppingDistance = keepDistance;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // The agent will now automatically stop 'keepDistance' units away
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            // Clear path if player escapes radius
            if (agent.hasPath) agent.ResetPath();
        }
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