using System;

//All the states that can be assigned to the Miner class

//  In this state the miner will walk to a goldmine and pick up a nugget
//  of gold. If the miner already has a nugget of gold he'll change state
//  to VisitBankAndDepositGold. If he gets thirsty he'll change state
//  to QuenchThirst
public class EnterMineAndDigForNugget : State<Miner>
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
            Console.WriteLine("Miner Bob: Walkin' to the goldmine");
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

        Console.WriteLine("Miner Bob: Pickin' up a nugget");

        //if enough gold mined, go and put it in the bank
        if (miner.PocketsFull())
        {
            miner.GetFSM().ChangeState(VisitBankAndDepositGold.Instance);
        }

        if (miner.Thirsty())
        {
            miner.GetFSM().ChangeState(QuenchThirst.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Miner Bob: Ah'm leavin' the goldmine with mah pockets full o' sweet gold");
    }

    public override bool OnMessage(Miner miner, Telegram telegram)
    {
        return false;
    }
}

//  Entity will go to a bank and deposit any nuggets he is carrying. If the 
//  miner is subsequently wealthy enough he'll walk home, otherwise he'll
//  keep going to get more gold
public class VisitBankAndDepositGold : State<Miner>
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
        Console.WriteLine("Miner Bob: Goin' to the bank. Yes siree");
        miner.ChangeLocation(location_type.bank);
    }

    public override void Execute(Miner miner)
    {
        //deposit the gold
        miner.AddToWealth(miner.GoldCarried);
        
        miner.GoldCarried = 0;

        Console.WriteLine("Miner Bob: Depositing gold. Total savings now: {0}", miner.Wealth);

        //wealthy enough to have a well earned rest?
        if (miner.Wealth >= Miner.ComfortLevel)
        {
            Console.WriteLine("Miner Bob: WooHoo! Rich enough for now. Back home to mah li'lle lady");
            miner.GetFSM().ChangeState(GoHomeAndSleepTilRested.Instance);
        }
        else
        {
            miner.GetFSM().ChangeState(EnterMineAndDigForNugget.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Miner Bob: Leavin' the bank");
    }

    public override bool OnMessage(Miner miner, Telegram telegram)
    {
        return false;
    }
}

//  miner will go home and sleep until his fatigue is decreased
//  sufficiently
public class GoHomeAndSleepTilRested : State<Miner>
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
            Console.WriteLine("Miner Bob: Walkin' home");
            miner.ChangeLocation(location_type.shack);
        }
    }

    public override void Execute(Miner miner)
    {
        if (!miner.Fatigued())
        {
            Console.WriteLine("Miner Bob: What a God darn fantastic nap! Time to find more gold");
            miner.GetFSM().ChangeState(EnterMineAndDigForNugget.Instance);
        }
        else
        {
            //sleep
            miner.DecreaseFatigue();
            Console.WriteLine("Miner Bob: ZZZZ... ");
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Miner Bob: Leaving the house");
    }

    public override bool OnMessage(Miner miner, Telegram telegram)
    {
        return false;
    }
}

public class QuenchThirst : State<Miner>
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
        if (miner.Location != location_type.saloon)
        {
            miner.ChangeLocation(location_type.saloon);
            Console.WriteLine("Miner Bob: Boy, ah sure is thusty! Walking to the saloon");
        }
    }

    public override void Execute(Miner miner)
    {
        if (miner.Thirsty())
        {
            miner.BuyAndDrinkAWhiskey();
            Console.WriteLine("Miner Bob: That's mighty fine sippin liquer");
            miner.GetFSM().ChangeState(EnterMineAndDigForNugget.Instance);
        }
    }

    public override void Exit(Miner miner)
    {
        Console.WriteLine("Miner Bob: Leaving the saloon, feelin' good");
    }

    public override bool OnMessage(Miner miner, Telegram telegram)
    {
        return false;
    }
}
