using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{

public class TSOFigureAction
{
    internal int frame_index;
    internal string motion_name;

    public TSOFigureAction(int frame_index, string motion_name)
    {
        this.frame_index = frame_index;
        this.motion_name = motion_name;
    }
}

public class TSOFigureMotion
{
    internal int frame_index = 0;
    internal LinkedList<TSOFigureAction> action_list = new LinkedList<TSOFigureAction>();
    internal LinkedListNode<TSOFigureAction> current_action = null;
    internal Dictionary<string, TMOFile> tmomap = new Dictionary<string, TMOFile>();

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

    public void LoadTMOFile(string source_file)
    {
        if (File.Exists(source_file))
        {
            TMOFile tmo = new TMOFile();
            tmo.Load(source_file);

            string name = Path.GetFileNameWithoutExtension(source_file);
            tmomap[name] = tmo;
        } else {
            Console.WriteLine("Error: file not found in LoadTMOFile: " + source_file);
        }
    }

    public TMOFile FindTMOFile(string name)
    {
        TMOFile tmo;
        if (tmomap.TryGetValue(name, out tmo))
            return tmo;
        else
            return null;
    }

    public TMOFile GetTMOFile()
    {
        TSOFigureAction act1 = FindAction1();
        TMOFile tmo1 = FindTMOFile(act1.motion_name);
        return tmo1;
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

    public TSOFigureAction FindAction1()
    {
        return current_action.Value;
    }

    public void Add(int frame_index, string motion_name)
    {
        LinkedListNode<TSOFigureAction> act = action_list.First;
        LinkedListNode<TSOFigureAction> new_act = new LinkedListNode<TSOFigureAction>(new TSOFigureAction(frame_index, motion_name));
        LinkedListNode<TSOFigureAction> found = null;

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
