using UnityEngine;
using UnityEngine.AI;

public class State_Alarm : IState
{
    private EnemyController owner;
    private NavMeshAgent agent;
    private Vector3 noisePosition;
    private bool hasReachedNoise;
    private float investigationTimer = 7f; 
    private float timer = 0f; 

    public State_Alarm(EnemyController enemy, Vector3 noisePos)
    {
        owner = enemy;
        noisePosition = noisePos;
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        Debug.Log("Entering Noise Investigation State");
        agent.speed = 10; 
        agent.isStopped = false;
        agent.SetDestination(noisePosition);
        hasReachedNoise = false;

        owner.animator.SetBool("isWalking", false);
        owner.animator.SetBool("isCrawling", true);
        owner.animator.SetBool("isIdle", false);
    }

    public void Execute()
    {
        if (!hasReachedNoise && Vector3.Distance(owner.transform.position, noisePosition) < 1.5f)
        {
            hasReachedNoise = true;
            agent.isStopped = true;
            owner.animator.SetBool("isCrawling", false);
            owner.animator.SetBool("isIdle", true);
            timer = 0f; 
        }

        if (hasReachedNoise)
        {
            owner.animator.SetBool("isCrawling", false);
            owner.animator.SetBool("isIdle", true);
            timer += Time.deltaTime;

            if (timer >= investigationTimer)
            {
                owner.animator.SetBool("isWalking", true);
                owner.animator.SetBool("isCrawling", false);
                owner.animator.SetBool("isIdle", false);
                owner.stateMachine.ChangeState(new State_Patrol(owner));
            }
        }
    }

    public void Exit()
    {
        owner.animator.SetBool("isWalking", true);
        owner.animator.SetBool("isCrawling", false);
        owner.animator.SetBool("isInvestigating", false);

        agent.speed = 3.5f;
        Debug.Log("Exiting Noise Investigation State");
    }
}
