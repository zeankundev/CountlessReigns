using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool isLockedToPlayer = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isLockedToPlayer = !isLockedToPlayer;
        }

        if (isLockedToPlayer)
        {
            // transform.position = 
        }
        else
        {
            // Logic for free camera movement
            Debug.Log("Camera is in free mode.");
        }
    }
}
