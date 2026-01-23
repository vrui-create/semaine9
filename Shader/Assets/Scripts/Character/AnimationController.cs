using Unity.VisualScripting;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("animator")]
    [SerializeField] Animator animator;

    private int baseLayer;
    private int hitLayer;
    private int time;

    private void Start()
    {
        baseLayer = animator.GetLayerIndex("Movement");
        hitLayer = animator.GetLayerIndex("Hit");
        time = animator.GetLayerIndex("SlowTime");

        animator.SetLayerWeight(baseLayer, 1.0f);
        animator.SetLayerWeight(hitLayer, 1.0f);
        animator.SetLayerWeight(time, 1.0f);

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void Walk(bool walking)
    {
        animator.SetBool("IsWalking", walking);
    }

    public void Sprint(bool sprinting)
    {
        animator.SetBool("IsSprinting", sprinting);
    }

    public void Attack()
    {
        animator.SetTrigger("IsAttacking");
    }

    public void Hit()
    {
        animator.SetTrigger("Hit");
    }

    public void TimeAbility()
    {
        animator.SetTrigger("Time");
    }
}
