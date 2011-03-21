using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateMachine<entity_type>
{
    entity_type owner;
    
    State<entity_type> currentState;
    
    public StateMachine(entity_type owner)
    {
        this.owner = owner;
        this.currentState = null;
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Execute(owner);
    }

    public void ChangeState(State<entity_type> newState)
    {
        //make sure both states are both valid before attempting to 
        //call their methods
        Debug.Assert(newState != null, "<StateMachine::ChangeState>: trying to change to a null state");

        //call the exit method of the existing state
        currentState.Exit(owner);

        //change state to the new state
        currentState = newState;

        //call the entry method of the new state
        currentState.Enter(owner);
    }

    public State<entity_type> CurrentState { get { return currentState; } set { currentState = value; } }
}
