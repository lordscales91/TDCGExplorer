using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateMachine<entity_type>
{
    //a pointer to the agent that owns this instance
    entity_type owner;
    
    State<entity_type> currentState;
    
    //a record of the last state the agent was in
    State<entity_type> previousState;

    //this is called every time the FSM is updated
    State<entity_type> globalState;
    
    public StateMachine(entity_type owner)
    {
        this.owner = owner;
        this.currentState = null;
        this.previousState = null;
        this.globalState = null;
    }

    //call this to update the FSM
    public void Update()
    {
        //if a global state exists, call its execute method, else do nothing
        if (globalState != null)
            globalState.Execute(owner);

        //same for the current state
        if (currentState != null)
            currentState.Execute(owner);
    }

    public bool HandleMessage(Telegram telegram)
    {
        return false;
    }

    //change to a new state
    public void ChangeState(State<entity_type> newState)
    {
        //make sure both states are both valid before attempting to 
        //call their methods
        Debug.Assert(newState != null, "<StateMachine::ChangeState>: trying to change to a null state");

        //keep a record of the previous state
        previousState = currentState;

        //call the exit method of the existing state
        currentState.Exit(owner);

        //change state to the new state
        currentState = newState;

        //call the entry method of the new state
        currentState.Enter(owner);
    }

    //change state back to the previous state
    public void RevertToPreviousState()
    {
        ChangeState(previousState);
    }

    public State<entity_type> CurrentState { get { return currentState; } set { currentState = value; } }
    public State<entity_type> PreviousState { get { return previousState; } set { previousState = value; } }
    public State<entity_type> GlobalState { get { return globalState; } set { globalState = value; } }
}
