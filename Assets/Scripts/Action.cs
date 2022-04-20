using UnityEngine;

public class Action
{
    public enum Type
    {
        None,
        Stop,
        NavMove,
        Shoot,
        Pass,
        LobPass,
        Move,
        MoveTo,
        Tackle,
        Headbutt,
        Dribble,
        Throw,
        ChangePlayer
    }

    public readonly Type ActionType;
    public readonly Vector3 Direction;
    public readonly Vector3 Position;
    public readonly float Duration;
    public readonly float Force;

    public readonly bool DirectionalAction;
    public readonly bool WaitForRotation;

    public static Action None => new Action(Type.None);

    public static implicit operator bool(Action action) => action != null && action.ActionType != Type.None;

    private Action(Type type)
    {
        ActionType = type;

        DirectionalAction = false;
        WaitForRotation = false;
    }

    private Action(Type type, float force)
    {
        ActionType = type;
        Force = force;

        DirectionalAction = true;
        WaitForRotation = true;
    }

    private Action(Type type, Vector3 position)
    {
        ActionType = type;
        Position = position;

        DirectionalAction = false;
        WaitForRotation = false;
    }

    private Action(Type type, Vector3 direction, bool waitForRotation)
    {
        ActionType = type;
        Direction = direction;

        DirectionalAction = true;
        WaitForRotation = waitForRotation;
    }

    public static Action NavMove()
    {
        return new Action(Type.NavMove);
    }

    public static Action Stop()
    {
        return new Action(Type.Stop);
    }

    public static Action Shoot(float force)
    {
        return new Action(Type.Shoot, force);
    }

    public static Action Pass(Vector3 direction)
    {
        return new Action(Type.Pass, direction, true);
    }

    public static Action LobPass(Vector3 direction)
    {
        return new Action(Type.LobPass, direction, true);
    }

    public static Action Move(Vector3 direction)
    {
        return new Action(Type.Move, direction, false);
    }

    public static Action MoveTo(Vector3 position)
    {
        return new Action(Type.MoveTo, position);
    }

    public static Action Tackle(Vector3 direction)
    {
        return new Action(Type.Tackle, direction, true);
    }

    public static Action Headbutt(Vector3 direction)
    {
        return new Action(Type.Headbutt, direction, true);
    }

    public static Action Dribble()
    {
        return new Action(Type.Dribble);
    }

    public static Action Throw(Vector3 direction)
    {
        return new Action(Type.Throw, direction, true);
    }

    public static Action ChangePlayer()
    {
        return new Action(Type.ChangePlayer);
    }
}
