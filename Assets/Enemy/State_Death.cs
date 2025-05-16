
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
public class State_Death : IState
{
    private EnemyController owner;
    private NavMeshAgent agent;

    public State_Death(EnemyController owner)
    {
        this.owner = owner;
    }

    public void Enter()
    {
        Debug.Log("Entering Death State");

        agent = owner.GetComponent<NavMeshAgent>();
        agent.isStopped = true;  
        agent.enabled = false;    

        // Disable Animator
        if (owner.animator != null)
        {
            Object.Destroy(owner.animator);
        }
        //Object.Destroy(owner.transform.Find("monstercollider").GetComponent<BoxCollider>());
    }

    public void Execute()
    {

    }

    public void Exit()
    {
    }
}
