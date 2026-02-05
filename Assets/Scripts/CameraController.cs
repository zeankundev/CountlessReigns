using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private bool isLockedToPlayer = true;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    private Vector3 lastMousePosition;
    [SerializeField] private float dragSensitivity = 0.5f;
    private bool wasLockedLastFrame = true;
    [SerializeField] private float smoothTime = 0.15f; // Medium speed (lower is faster)
    private Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            isLockedToPlayer = !isLockedToPlayer;
            
            // Reset velocity when toggling to ensure a fresh start
            currentVelocity = Vector3.zero;
        }

        if (isLockedToPlayer)
        {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, -1f);
            
            // SmoothDamp provides a natural "Ease Out"
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPos, 
                ref currentVelocity, 
                smoothTime
            );
        }
        else
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.isPressed)
            {
                Vector3 currentMousePos = mouse.position.ReadValue();
                Vector3 worldDelta = Camera.main.ScreenToWorldPoint(currentMousePos) - Camera.main.ScreenToWorldPoint(lastMousePosition);
                transform.position -= worldDelta * dragSensitivity;
                transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            }

            lastMousePosition = mouse.position.ReadValue();
        }

        wasLockedLastFrame = isLockedToPlayer;
    }
}
