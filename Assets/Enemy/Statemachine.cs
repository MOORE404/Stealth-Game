using System.Diagnostics;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class StateMachine
{
    public IState currentState;
    
    public void ChangeState(IState newState)
    {
        if (currentState is State_Death) return;

        if (currentState is State_Attack && !(newState is State_Death))
        {
            return;
        }
        if (currentState != null)
            currentState.Exit();
        
        currentState = newState;
        currentState.Enter();
    }
    
    public void Update()
    {
        if (currentState != null)
            currentState.Execute();
    }
}
