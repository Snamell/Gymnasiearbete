using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelareHorisontellRörelse : StateMachineBehaviour
{

    public float runSpeed = 10f;

    float moveX;


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moveX = animator.GetFloat(SAP.moveX);

        animator.SetFloat(SAP.forceX, moveX * runSpeed);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(SAP.forceX, 0);
    }

}
