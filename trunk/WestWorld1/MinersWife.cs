using System;
using System.Collections.Generic;
using System.Diagnostics;

class MinersWife : BaseGameEntity
{
    //an instance of the state machine class
    StateMachine<MinersWife> stateMachine;
    
    location_type location;
    
    public MinersWife(int id) : base(id)
    {
        stateMachine = new StateMachine<MinersWife>(this);
        stateMachine.CurrentState = DoHouseWork.Instance;
        stateMachine.GlobalState = WifesGlobalState.Instance;
    }

    public location_type Location 
    { 
        get { return location; } 
    }

    public void ChangeLocation(location_type loc)
    {
        location = loc;
    }

    public override void Update()
    {
        stateMachine.Update();
    }

    public StateMachine<MinersWife> GetFSM() 
    { 
        return stateMachine; 
    }
}
