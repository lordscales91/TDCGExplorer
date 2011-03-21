using System;

//All the states that can be assigned to the Miner class

//  In this state the miner will walk to a goldmine and pick up a nugget
//  of gold. If the miner already has a nugget of gold he'll change state
//  to VisitBankAndDepositGold. If he gets thirsty he'll change state
//  to QuenchThirst
public class EnterMineAndDigForNugget : State
{
    static readonly EnterMineAndDigForNugget instance = new EnterMineAndDigForNugget();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static EnterMineAndDigForNugget() { }

    EnterMineAndDigForNugget() { }

    //this is a singleton
    public static EnterMineAndDigForNugget Instance { get { return instance; } }
    
    public override void Enter(Miner miner)
    {
        if (miner.Location != location_type.goldmine)
        {
            Console.WriteLine("Walkin' to the goldmine");
            miner.ChangeLocation(location_type.goldmine);
        }
    }

    public override void Execute(Miner miner)
    {
        //the miner digs for gold until he is carrying in excess of MaxNuggets. 
        //If he gets thirsty during his digging he packs up work for a while and 
        //changes state to go to the saloon for a whiskey.
        miner.AddToGoldCarried(1);

        miner.IncreaseFatigue();

        Console.WriteLine("Pickin' up a nugget");

        //if enough gold mined, go and put it in the bank
        if (miner.PocketsFull())
        {
            miner.ChangeState(VisitBankAndDepositGold.Instance);
        }

        if (miner.Thirsty())
        {
            miner.ChangeState(QuenchThirst.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Ah'm leavin' the goldmine with mah pockets full o' sweet gold");
    }
}

//  Entity will go to a bank and deposit any nuggets he is carrying. If the 
//  miner is subsequently wealthy enough he'll walk home, otherwise he'll
//  keep going to get more gold
public class VisitBankAndDepositGold : State
{
    static readonly VisitBankAndDepositGold instance = new VisitBankAndDepositGold();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static VisitBankAndDepositGold() { }

    VisitBankAndDepositGold() { }

    //this is a singleton
    public static VisitBankAndDepositGold Instance { get { return instance; } }
    
    public override void Enter(Miner miner)
    {
        Console.WriteLine("Goin' to the bank. Yes siree");
    }

    public override void Execute(Miner miner)
    {
        //deposit the gold
        miner.AddToWealth(miner.GoldCarried);
        
        miner.GoldCarried = 0;

        Console.WriteLine("Depositing gold. Total savings now: {0}", miner.Wealth);

        //wealthy enough to have a well earned rest?
        if (miner.Wealth >= Miner.ComfortLevel)
        {
            Console.WriteLine("WooHoo! Rich enough for now. Back home to mah li'lle lady");
            miner.ChangeState(GoHomeAndSleepTilRested.Instance);
        }
        else
        {
            miner.ChangeState(EnterMineAndDigForNugget.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Leavin' the bank");
    }
}

//  miner will go home and sleep until his fatigue is decreased
//  sufficiently
public class GoHomeAndSleepTilRested : State
{
    static readonly GoHomeAndSleepTilRested instance = new GoHomeAndSleepTilRested();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static GoHomeAndSleepTilRested() { }

    GoHomeAndSleepTilRested() { }

    //this is a singleton
    public static GoHomeAndSleepTilRested Instance { get { return instance; } }
    
    public override void Enter(Miner miner)
    {
        if (miner.Location != location_type.shack)
        {
            Console.WriteLine("Walkin' home");
            miner.ChangeLocation(location_type.shack);
        }
    }

    public override void Execute(Miner miner)
    {
        if (!miner.Fatigued())
        {
            Console.WriteLine("What a God darn fantastic nap! Time to find more gold");
            miner.ChangeState(EnterMineAndDigForNugget.Instance);
        }
        else
        {
            //sleep
            miner.DecreaseFatigue();
            Console.WriteLine("ZZZZ... ");
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Leaving the house");
    }
}

public class QuenchThirst : State
{
    static readonly QuenchThirst instance = new QuenchThirst();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static QuenchThirst() { }

    QuenchThirst() { }

    //this is a singleton
    public static QuenchThirst Instance { get { return instance; } }
    
    public override void Enter(Miner miner)
    {
        Console.WriteLine("Boy, ah sure is thusty! Walking to the saloon");
    }

    public override void Execute(Miner miner)
    {
        if (miner.Thirsty())
        {
            miner.BuyAndDrinkAWhiskey();
            Console.WriteLine("That's mighty fine sippin liquer");
            miner.ChangeState(EnterMineAndDigForNugget.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Leaving the saloon, feelin' good");
    }
}
