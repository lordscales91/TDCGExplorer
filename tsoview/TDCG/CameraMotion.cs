using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;

namespace TDCG
{

public class CameraAction
{
    internal int frame_index;
    internal Vector3 eye;
    internal Vector3 center;
    internal int interp_length;

    public CameraAction(int frame_index, Vector3 eye, Vector3 center, int interp_length)
    {
        this.frame_index = frame_index;
        this.eye = eye;
        this.center = center;
        this.interp_length = interp_length;
    }

    //ï‚ä‘ÇäJénÇ∑ÇÈéûçèÇìæÇ‹Ç∑ÅB
    public int interp_begin
    {
        get {
            return frame_index - interp_length;
        }
    }
}

public class CameraMotion
{
    internal int frame_index = 0;
    internal LinkedList<CameraAction> action_list = new LinkedList<CameraAction>();
    internal LinkedListNode<CameraAction> current_action = null;
    internal Camera camera = null;

    public CameraMotion(Camera camera)
    {
        this.camera = camera;
    }

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

    public void UpdateCamera()
    {
        CameraAction act1 = FindAction1();
        CameraAction act2 = FindAction2();

        if (frame_index == act1.frame_index)
        {
            //Console.WriteLine("camera LookAt index {0}", frame_index);
            camera.LookAt(act1.eye, act1.center);
        }
        if (act2 != null && frame_index >= act2.interp_begin)
        {
            Camera cam1 = new Camera();
            Camera cam2 = new Camera();
            cam1.LookAt(act1.eye, act1.center);
            cam2.LookAt(act2.eye, act2.center);
            int frame_delta = frame_index - act2.interp_begin;
            //Console.WriteLine("camera Interp index {0} delta {1}", frame_index, frame_delta);
            camera.Interp(cam1, cam2, (float)frame_delta/(float)act2.interp_length);
        }
    }

    public void NextFrame()
    {
        UpdateCamera();
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

    public CameraAction FindAction1()
    {
        return current_action.Value;
    }

    public CameraAction FindAction2()
    {
        if (current_action.Next != null)
            return current_action.Next.Value;
        else
            return null;
    }

    public void Add(int frame_index, Vector3 eye, Vector3 center)
    {
        Add(frame_index, eye, center, 0);
    }
    public void Add(int frame_index, Vector3 eye, Vector3 center, int interp_length)
    {
        LinkedListNode<CameraAction> act = action_list.First;
        LinkedListNode<CameraAction> new_act = new LinkedListNode<CameraAction>(new CameraAction(frame_index, eye, center, interp_length));
        LinkedListNode<CameraAction> found = null;

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
