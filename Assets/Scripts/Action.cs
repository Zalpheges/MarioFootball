using UnityEngine;

public class Action
{
    public enum Type
    {
        None,
        Shoot,
        Pass,
        LobPass,
        Move,
        Tackle,
        Headbutt,
        Dribble,
        Throw,
        ChangePlayer
    }

    public readonly Type ActionType;
    public readonly Vector3 Direction;
    public readonly float Duration;
    public readonly float Force;

    public readonly bool DirectionnalAction;

    public static Action None => new Action(Type.None);

    public static implicit operator bool(Action action) => action != null && action.ActionType != Type.None;

    private Action(Type type)
    {
        ActionType = type;

        DirectionnalAction = false;
    }

    private Action(Type type, Vector3 direction, float force = 0f)
    {
        ActionType = type;
        Direction = direction;
        Force = force;

        DirectionnalAction = true;
    }

    public static Action Shoot(Vector3 direction, float force)
    {
        return new Action(Type.Shoot, direction, force);
    }

    public static Action Pass(Vector3 direction)
    {
        return new Action(Type.Pass, direction);
    }

    public static Action Move(Vector3 direction)
    {
        return new Action(Type.Move, direction);
    }

    public static Action Tackle(Vector3 direction)
    {
        return new Action(Type.Tackle, direction);
    }

    public static Action Headbutt(Vector3 direction)
    {
        return new Action(Type.Headbutt, direction);
    }

    public static Action Dribble()
    {
        return new Action(Type.Dribble);
    }

    public static Action Throw(Vector3 direction)
    {
        return new Action(Type.Throw, direction);
    }

    public static Action ChangePlayer()
    {
        return new Action(Type.ChangePlayer);
    }
}
