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

    void Update()
    {
        // ── 1. Retry finding UIBridge until the Canvas exists in the scene ──
        if (uiBridge == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
                uiBridge = canvas.GetComponent<UIBridge>();
        }

        // ── 2. Only run camera logic while MainGameplay is active ──
        GameObject mainGameplay = GameObject.Find("MainGameplay");
        if (mainGameplay == null || !mainGameplay.activeInHierarchy) return;

        // ── 3. Retry finding the player until it spawns ──
        if (player == null)
            player = GameObject.Find("MainGame/Player");

        // ── 4. Lock-toggle input ──
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            isLockedToPlayer = !isLockedToPlayer;
            uiBridge?.UpdateCamLockStatus(isLockedToPlayer); // null-safe call
            currentVelocity = Vector3.zero;
        }

        // ── 5. Camera behaviour ──
        if (isLockedToPlayer)
        {
            if (player == null) return; // player not spawned yet — stay put

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