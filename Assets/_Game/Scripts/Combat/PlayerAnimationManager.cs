using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void UpdateMove(float Speed)
    {
        animator.SetFloat("MoveSpeed",Speed);
    }

    public void ReturnToBase()
    {
        animator.SetTrigger("Base");
    }

    public void PlayJump()
    {
        animator.SetTrigger("Jump");
    }


}
