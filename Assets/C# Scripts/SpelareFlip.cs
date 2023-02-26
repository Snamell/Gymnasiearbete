using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skapar en klass vid namn SpelareFlip som ger tillgång till att göra förändringar till spelarens animationer i scriptet och i Unity. 
public class SpelareFlip : StateMachineBehaviour
{

    // Skapar en funktion för att bestämma om spelaren rör sig vänster eller höger. 
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float moveX = animator.GetFloat(SAP.moveX);

        // Om moveX == 0 rör sig inte spelaren, och därför görs inga förändringar på spelarens modell. Om moveX är större än 0 rör sig spelaren åt höger och då riktas karaktären 
        // åt höger. Om moveX är mindre än 0 rör sig spelaren åt vänster och då riktas karaktären åt vänster.

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
