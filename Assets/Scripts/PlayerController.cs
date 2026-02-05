using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject idk;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || Camera.main == null) return;

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
                Destroy(thingy, 0.2f);
            }
            else
            {
                Debug.Log("No collider hit");
            }
        }
    }
}
