//abstract base class to define an interface for a state
abstract public class State<entity_type>
{
    //this will execute when the state is entered
    abstract public void Enter(entity_type entity);

    //this is the state's normal update function
    abstract public void Execute(entity_type entity);

    //this will execute when the state is exited
    abstract public void Exit(entity_type entity);

    abstract public bool OnMessage(entity_type entity, Telegram telegram);
}
