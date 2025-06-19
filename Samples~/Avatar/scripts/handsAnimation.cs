using UnityEngine;

public class HandsAnimation : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void GrabAnimation()
    {
        animator.SetInteger("PULGAR", 1);
        animator.SetInteger("INDICE", 1);
        animator.SetInteger("CORAZON", 1);
        animator.SetInteger("ANULAR", 1);
        animator.SetInteger("MENIQUE", 1);
    }

    public void SelectAnimation()
    {
        animator.SetInteger("PULGAR", 2);
        animator.SetInteger("INDICE", 2);
        animator.SetInteger("CORAZON", 1);
        animator.SetInteger("ANULAR", 1);
        animator.SetInteger("MENIQUE", 1);
    }

    public void HandIdleAnimation()
    {
        animator.SetInteger("PULGAR", 0);
        animator.SetInteger("INDICE", 0);
        animator.SetInteger("CORAZON", 0);
        animator.SetInteger("ANULAR", 0);
        animator.SetInteger("MENIQUE", 0);
    }


}
