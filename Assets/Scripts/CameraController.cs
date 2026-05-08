using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private bool isLockedToPlayer = true;
    private GameObject player;

    private Vector3 lastMousePosition;
    private UIBridge uiBridge;
    [SerializeField] private float dragSensitivity = 0.5f;
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 currentVelocity = Vector3.zero;

    // No player search in Start — it may not exist during the intro.
    void Start()
    {
        uiBridge = GameObject.Find("Canvas").GetComponent<UIBridge>();
    }

    void Update()
    {
        // Keep trying to find the player until it exists in the scene.
        if (player == null)
        {
            player = GameObject.Find("MainGame/Player");
        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            isLockedToPlayer = !isLockedToPlayer;
            uiBridge.UpdateCamLockStatus(isLockedToPlayer);
            currentVelocity = Vector3.zero;
        }

        if (isLockedToPlayer)
        {
            // Nothing to follow yet — stay put until the player exists.
            if (player == null) return;

            Vector3 targetPos = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                -1f
            );

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
                Vector3 worldDelta =
                    Camera.main.ScreenToWorldPoint(currentMousePos) -
                    Camera.main.ScreenToWorldPoint(lastMousePosition);

                transform.position -= worldDelta * dragSensitivity;
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -1f
                );
            }

            lastMousePosition = mouse.position.ReadValue();
        }
    }
}