using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class State_Suspicious : IState
{
    private EnemyController enemy;
    private float patrolRadius = 7f;
    private float minDistance = 5f;
    private Vector3 patrolTarget;
    private float idleTime = 2f; 
    private bool isIdling = false;

    public State_Suspicious(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("grumgrum");
        SetNewPatrolPoint();
    }

    public void Execute()
    {
        if (isIdling) return; 

        if (enemy.agent.remainingDistance <= 0.5f && !enemy.agent.pathPending) 
        {
            enemy.StartCoroutine(IdleBeforeMoving()); 
        }
    }

    public void Exit()
    {
        enemy.agent.ResetPath();
    }

private IEnumerator IdleBeforeMoving()
{
    isIdling = true;
    enemy.agent.isStopped = true; // Stop movement
    enemy.animator.SetBool("isWalking", false);
    enemy.animator.SetBool("isIdle", true);
    enemy.animator.SetBool("isRunning", false);
    yield return new WaitForSeconds(idleTime);

    isIdling = false;
    enemy.agent.isStopped = false;
    SetNewPatrolPoint();
}
    
private void SetNewPatrolPoint()
{
    if (enemy.target == null) return;

    Vector3 randomDirection;
    float distance;
    int attempts = 10;
    do
    {
        randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0;
        patrolTarget = enemy.target.position + randomDirection;
        distance = Vector3.Distance(patrolTarget, enemy.target.position);
        attempts--;
    } while (distance < minDistance && attempts > 0);

    if (attempts > 0)
    {
        enemy.agent.SetDestination(patrolTarget);
        enemy.agent.isStopped = false; // Ensure agent is moving
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isIdle", false);
        enemy.animator.SetBool("isRunning", false);
    }
    else
    {
        Debug.LogWarning("Failed to find a valid patrol point!");
    }
}
}
