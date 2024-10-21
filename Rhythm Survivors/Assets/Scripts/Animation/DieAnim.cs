using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.transform.tag == "Player")
        {
            // Alert the game that the player has died
            PauseMenu.isAlive = false;
        }
        else if (animator.transform.tag == "Enemy")
        {
            Debug.Log("TEST");
            // Destroy the enemy game object
            Destroy(animator.gameObject);
        }
    }
}