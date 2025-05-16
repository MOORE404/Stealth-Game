using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class State_Look : IState
{
    private EnemyController owner; 
    private NavMeshAgent agent;
    private float lookDelay = .5f; 
    private float returnToPatrolDelay = 3f; 

    public State_Look(EnemyController owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        
        owner.animator.SetBool("isIdle", true);
        owner.animator.SetBool("isWalking", false);
        owner.animator.SetBool("isInvestigating", false);

        
        owner.suspicionLevel = 0;

        
        agent = owner.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        
        owner.StartCoroutine(LookAfterDelay());
    }

    public void Execute()
    {
        Debug.Log("LOOKING");
        if (owner.CanSeePlayer()) 
        {
            Debug.Log("Player spotted! Switching to Attack.");
            owner.stateMachine.ChangeState(new State_Attack(owner));
            return;
        }
        else{
            
            owner.StartCoroutine(ReturnToPatrol());
        }
    }

    public void Exit()
    {
        owner.animator.SetBool("isIdle", false);
        owner.animator.SetBool("isWalking", true);
    }

    private IEnumerator LookAfterDelay()
    {
        yield return new WaitForSeconds(lookDelay); 
        yield return owner.StartCoroutine(RotateTowards(owner.lastSeenPosition)); 
    }

    private IEnumerator RotateTowards(Vector3 position)
    {
        Vector3 direction = (position - owner.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        while (Quaternion.Angle(owner.transform.rotation, targetRotation) > 0.1f) 
        {
            owner.transform.rotation = Quaternion.RotateTowards(
                owner.transform.rotation, 
                targetRotation, 
                Time.deltaTime * 120f 
            );

            yield return null;
        }
    }

    private IEnumerator ReturnToPatrol()
    {
        yield return new WaitForSeconds(returnToPatrolDelay); 
        owner.stateMachine.ChangeState(new State_Patrol(owner)); 
    }
}
