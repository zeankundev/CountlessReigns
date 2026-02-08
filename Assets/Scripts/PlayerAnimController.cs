using UnityEngine;

[RequireComponent(typeof(Animator))]

public class PlayerAnimController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void SetWalkBoolean(bool isWalking)
    {
        animator.SetBool("Is Walking", isWalking);
    }
}
