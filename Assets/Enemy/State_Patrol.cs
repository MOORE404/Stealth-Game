using UnityEngine;
using UnityEngine.AI;

public class State_Patrol : IState
{
    private EnemyController owner;
    private NavMeshAgent agent;
    private waypoint waypoint;
    private UnderSurfaceCheck underSurfaceCheck;

    public State_Patrol(EnemyController owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        waypoint = owner.waypoint;
        agent = owner.GetComponent<NavMeshAgent>();
        agent.speed = 2f;
        agent.destination = waypoint.transform.position;
        agent.isStopped = false;

        
        owner.animator.SetBool("isWalking", true);
        underSurfaceCheck = owner.GetComponent<UnderSurfaceCheck>(); 
    }

    public void Execute()
    {

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (waypoint.nextWaypoint != null)
            {
                waypoint = waypoint.nextWaypoint;
                agent.destination = waypoint.transform.position;
            }
        }

        if (owner.seenTarget)
        {
            owner.stateMachine.ChangeState(new State_Attack(owner));
        }

        // Check if the enemy is near a low surface using UnderSurfaceCheck
        if (underSurfaceCheck.IsNearLowSurface)  
        {
            owner.animator.SetBool("isCrawling", true); 
            owner.animator.SetBool("isWalking", false);  
        }
        else
        {
            owner.animator.SetBool("isCrawling", false);  
            owner.animator.SetBool("isWalking", true);    
        }
    }

    public void Exit()
    {
    }
}
