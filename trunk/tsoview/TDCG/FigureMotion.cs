using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TDCG
{

public class FigureAction
{
    internal int frame_index;
    internal TMOFile tmo;

    public FigureAction(int frame_index, TMOFile tmo)
    {
        this.frame_index = frame_index;
        this.tmo = tmo;
    }
}

public class FigureMotion
{
    internal int frame_index = 0;
    internal LinkedList<FigureAction> action_list = new LinkedList<FigureAction>();
    internal LinkedListNode<FigureAction> current_action = null;

    public int Count
    {
        get {
            return action_list.Count;
        }
    }

    public int Length
    {
        get {
            if (action_list.Last != null)
                return action_list.Last.Value.frame_index;
            else
                return 0;
        }
    }

    public TMOFile GetTMO()
    {
        FigureAction act = current_action.Value;
        return act.tmo;
    }

    public void NextFrame()
    {
        frame_index++;
        if (current_action.Next != null)
        {
            if (current_action.Next.Value.frame_index <= frame_index)
                current_action = current_action.Next;
        } else {
            frame_index = 0;
            current_action = action_list.First;
        }
    }

    public void Add(int frame_index, TMOFile tmo)
    {
        LinkedListNode<FigureAction> act = action_list.First;
        LinkedListNode<FigureAction> new_act = new LinkedListNode<FigureAction>(new FigureAction(frame_index, tmo));
        LinkedListNode<FigureAction> found = null;

        while (act != null)
        {
            if (act.Value.frame_index <= frame_index)
                found = act;
            else
                break;
            act = act.Next;
        }
        if (found != null)
            action_list.AddAfter(found, new_act);
        else {
            action_list.AddFirst(new_act);
            current_action = new_act;
        }
    }
}
}
