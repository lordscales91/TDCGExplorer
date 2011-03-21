//abstract base class to define an interface for a state
abstract public class State
{
    //this will execute when the state is entered
    abstract public void Enter(Miner miner);

    //this is the state's normal update function
    abstract public void Execute(Miner miner);

    //this will execute when the state is exited
    abstract public void Exit(Miner miner);
}
