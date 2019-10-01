using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEvents : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetIdleTrue()
    {
        animator.SetBool("idle", true);
    }
    public void SetIdleFalse ()
    {
        animator.SetBool("idle", false);
    }
}
