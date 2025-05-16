using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class State_Attack : IState
{
    
    private EnemyController owner;
    private NavMeshAgent agent;
    private Vector3 lastKnownPosition;
    private PlayerController playerController;
    private UnderSurfaceCheck underSurfaceCheck;

    public State_Attack(EnemyController owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Debug.Log("Entering Attack State");
        agent = owner.GetComponent<NavMeshAgent>();
        agent.speed = 7; 
        agent.isStopped = false;
        playerController = owner.target.GetComponent<PlayerController>();


        lastKnownPosition = owner.target.position; 
        underSurfaceCheck = owner.GetComponent<UnderSurfaceCheck>(); 
        

        // Set the animator to play the attack animation
        owner.animator.SetBool("isWalking", false);
        owner.animator.SetBool("isRunning", true);
        owner.animator.SetBool("isIdle", false);
        owner.animator.SetBool("isInvestigating", false);
    }

    public void Execute()
    {
        lastKnownPosition = owner.target.position; 
        agent.destination = lastKnownPosition; 
        // Check if the enemy is near a low surface using UnderSurfaceCheck
        if (underSurfaceCheck.IsNearLowSurface)  
        {
            agent.speed = 5;
            owner.animator.SetBool("isCrawling", true); 
            owner.animator.SetBool("isRunning", false);  
        }
        else
        {
            agent.speed = 7;
            owner.animator.SetBool("isCrawling", false);  
            owner.animator.SetBool("isRunning", true);    
        }

    }

    public void Exit()
    {

    }
}
