using System;
using System.Collections.Generic;
using System.Diagnostics;

//A class defining a goldminer.
public class Miner : BaseGameEntity
{
    //the amount of gold a miner must have before he feels comfortable
    public const int ComfortLevel = 5;
    //the amount of nuggets a miner can carry
    public const int MaxNuggets = 3;
    //above this value a miner is thirsty
    public const int ThirstLevel = 5;
    //above this value a miner is sleepy
    public const int TirednessThreshold = 5;

    State currentState;

    location_type location;

    public location_type Location
    {
        get { return location; }
    }

    public void ChangeLocation(location_type loc)
    {
        location = loc;
    }

    //how many nuggets the miner has in his pockets
    int goldCarried;

    int moneyInBank;

    //the higher the value, the thirstier the miner
    int thirst;

    //the higher the value, the more tired the miner
    int fatigue;

    public Miner(int id) : base(id)
    {
        currentState = new GoHomeAndSleepTilRested();
    }

    public void ChangeState(State newState)
    {
        //make sure both states are both valid before attempting to 
        //call their methods
        Debug.Assert(currentState != null && newState != null);

        //call the exit method of the existing state
        currentState.Exit(this);

        //change state to the new state
        currentState = newState;

        //call the entry method of the new state
        currentState.Enter(this);
    }

    public int GoldCarried
    {
        get { return goldCarried; }
        set { goldCarried = value; }
    }

    public void AddToGoldCarried(int val)
    {
        goldCarried += val;
        if (goldCarried < 0) goldCarried = 0;
    }

    public bool PocketsFull()
    {
        return goldCarried >= MaxNuggets;
    }

    public void AddToWealth(int val)
    {
        moneyInBank += val;
        if (moneyInBank < 0) moneyInBank = 0;
    }

    public int Wealth
    {
        get { return moneyInBank; }
        set { moneyInBank = value; }
    }

    public bool Thirsty()
    {
        if (thirst >= ThirstLevel)
        {
            return true;
        }
        return false;
    }

    public void BuyAndDrinkAWhiskey()
    {
        thirst = 0; moneyInBank -= 2;
    }

    //this must be implemented
    public override void Update()
    {
        thirst += 1;

        if (currentState != null)
        {
            currentState.Execute(this);
        }
    }

    public bool Fatigued()
    {
        if (fatigue > TirednessThreshold)
        {
            return true;
        }
        return false;
    }

    public void DecreaseFatigue()
    {
        fatigue -= 1;
    }

    public void IncreaseFatigue()
    {
        fatigue += 1;
    }
}
