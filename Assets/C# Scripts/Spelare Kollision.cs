using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelareCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(" Non-trigger Colliders entering Collision");

    }
    void OnCollisionStay2D(Collision2D col)
    {
        Debug.Log(" Non-trigger Colliders in Collision");
    }    
}