using System;

class WifesGlobalState : State<MinersWife>
{
    static readonly WifesGlobalState instance = new WifesGlobalState();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static WifesGlobalState() { }

    WifesGlobalState() { }

    //this is a singleton
    public static WifesGlobalState Instance { get { return instance; } }

    Random random = new Random();

    public override void Enter(MinersWife wife)
    {
    }

    public override void Execute(MinersWife wife)
    {
        if (random.NextDouble() < 0.1)
        {
            wife.GetFSM().ChangeState(VisitBathroom.Instance);
        }
    }

    public override void Exit(MinersWife wife)
    {
    }

    public override bool OnMessage(MinersWife entity, Telegram telegram)
    {
        return false;
    }
}

class DoHouseWork : State<MinersWife>
{
    static readonly DoHouseWork instance = new DoHouseWork();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static DoHouseWork() { }

    DoHouseWork() { }

    //this is a singleton
    public static DoHouseWork Instance { get { return instance; } }

    Random random = new Random();

    public override void Enter(MinersWife wife)
    {
    }

    public override void Execute(MinersWife wife)
    {
        switch (random.Next(3))
        {
            case 0:
                Console.WriteLine("Elsa: Moppin' the floor");
                break;
            case 1:
                Console.WriteLine("Elsa: Washin' the dishes");
                break;
            case 2:
                Console.WriteLine("Elsa: Makin' the bed");
                break;
        }
    }

    public override void Exit(MinersWife wife)
    {
    }

    public override bool OnMessage(MinersWife wife, Telegram telegram)
    {
        return false;
    }
}   

class VisitBathroom : State<MinersWife>
{
    static readonly VisitBathroom instance = new VisitBathroom();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static VisitBathroom() { }

    VisitBathroom() { }

    //this is a singleton
    public static VisitBathroom Instance { get { return instance; } }

    public override void Enter(MinersWife wife)
    {
        Console.WriteLine("Elsa: Walkin' to the can. Need to powda mah pretty li'lle nose");
    }

    public override void Execute(MinersWife wife)
    {
        Console.WriteLine("Elsa: Ahhhhhh! Sweet relief!");
        wife.GetFSM().RevertToPreviousState();
    }

    public override void Exit(MinersWife wife)
    {
        Console.WriteLine("Elsa: Leavin' the Jon");
    }

    public override bool OnMessage(MinersWife wife, Telegram telegram)
    {
        return false;
    }
}
