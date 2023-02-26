using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skapar en klass med namnet SpelareHorisontellRörelse som ger tillgång till förändringar för spelarens hastighet i scriptet och i Unity. 
public class SpelareHorisontellRörelse : StateMachineBehaviour
{

    // Skapar ett floatvärde för runSpeed som kan sättas i både scriptet och i Unity. 
    public float runSpeed = 10f;

    // Skriver över en funktion från "statemachinebehaviour" som körs när "animationsstaten" förändras. 
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(SAP.forceX, animator.GetFloat(SAP.moveX) * runSpeed);
    }

    // Skriver över en funktion från "statemachinebehaviour" som körs när en "animationsstate" tar slut. 
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat(SAP.forceX, 0);
    }

}
