using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Action
{
    public enum ActionType
    {
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

    public ActionType type;
    public Vector2 deltaMove;
    public float shootForce;
    public Vector3 direction;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 bezierPoint;
    public float duration;
    public bool isSprinting;

    public static Action Shoot(float shootForce, Vector3 direction, Vector3 startPosition, float duration)
    {
        Action action = new Action();
        action.type = ActionType.Shoot;
        action.direction = direction;
        action.startPosition = startPosition;
        action.duration = duration;

        return action;
    }

    public static Action Pass(Vector3 direction, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        Action action = new Action();
        action.type = ActionType.Pass;
        action.direction = direction;
        action.startPosition = startPosition;
        action.endPosition = endPosition;
        action.duration = duration;

        return action;
    }

    public static Action Pass(Vector3 direction, Vector3 startPosition, Vector3 endPosition, Vector3 bezeierPoint, float duration)
    {
        Action action = Action.Pass(direction, startPosition, endPosition, duration);
        action.type = ActionType.LobPass;
        action.bezierPoint = bezeierPoint;

        return action;
    }

    public static Action Move(Vector3 direction)
    {
        Action action = new Action();
        action.type = ActionType.Move;
        action.direction = direction;

        return action;
    }

    public static Action Tackle(Vector3 direction)
    {
        Action action = new Action();
        action.type = ActionType.Tackle;
        action.direction = direction;

        return action;
    }

    public static Action HeadButt(Vector3 direction)
    {
        Action action = new Action();
        action.type = ActionType.Headbutt;
        action.direction = direction;

        return action;
    }

    public static Action Dribble()
    {
        Action action = new Action();
        action.type = ActionType.Dribble;

        return action;
    }

    public static Action Throw(Vector3 direction)
    {
        Action action = new Action();
        action.type = ActionType.Throw;
        action.direction = direction;

        return action;
    }

    public static Action ChangePlayer()
    {
        Action action = new Action();
        action.type = ActionType.ChangePlayer;

        return action;
    }
}
