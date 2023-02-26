using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelareFlip : StateMachineBehaviour
{

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float moveX = animator.GetFloat(SAP.moveX);

        if (moveX == 0)
        {

        }

        else if (moveX > 0)
        {
            animator.transform.localScale = new Vector3(1, 1, 1);
        }

        else if (moveX < 0)
        {
            animator.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

}
