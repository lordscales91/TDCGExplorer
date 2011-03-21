using System.Diagnostics;

//Base class for a game object
abstract public class BaseGameEntity
{
    //every entity must have a unique identifying number
    int id;

    //this is the next valid ID. Each time a BaseGameEntity is instantiated
    //this value is updated
    static int nextValidID = 0;

    public BaseGameEntity(int id)
    {
        this.ID = id;
    }

    //all entities must implement an update function
    abstract public void Update();

    public int ID
    {
        get { return id; }

        //this must be called within the constructor to make sure the ID is set
        //correctly. It verifies that the value passed to the method is greater
        //or equal to the next valid ID, before setting the ID and incrementing
        //the next valid ID
        set
        {
            Debug.Assert(value >= nextValidID, "<BaseGameEntity::SetID>: invalid ID");
            id = value;
            nextValidID = id + 1;
        }
    }
}
