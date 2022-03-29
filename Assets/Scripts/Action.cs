using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public enum Type
    {
        NavMove,
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

    public Type ActionType { get; private set; }
    public Vector3 DeltaMove { get; private set; }
    public float ShootForce { get; private set; }
    public Vector3 ShootDirection { get; private set; }
    public Vector3 StartPosition { get; private set; }
    public Vector3 EndPosition { get; private set; }
    public Vector3 BezierPoint { get; private set; }
    public float Duration { get; private set; }
    public bool IsSprinting { get; private set; }

    public static Action NavMove()
    {
        return new Action
        {
            ActionType = Type.NavMove
        };
    }
    public static Action Shoot(float shootForce, Vector3 direction, Vector3 startPosition, int duration)
    {
        return new Action
        {
            ActionType = Type.Shoot,
            ShootForce = shootForce,
            ShootDirection = direction,
            StartPosition = startPosition,
            Duration = duration
        };
    }

    public static Action Pass(Vector3 direction, Vector3 startPosition, Vector3 endPosition, int duration)
    {
        return new Action
        {
            ActionType = Type.Pass,
            ShootDirection = direction,
            StartPosition = startPosition,
            EndPosition = endPosition,
            Duration = duration
        };
    }

    public static Action Pass(Vector3 direction, Vector3 startPosition, Vector3 endPosition, Vector3 bezierPoint, int duration)
    {
        return new Action
        {
            ActionType = Type.Pass,
            ShootDirection = direction,
            StartPosition = startPosition,
            EndPosition = endPosition,
            BezierPoint = bezierPoint,
            Duration = duration
        };
    }

    public static Action Move(Vector3 direction)
    {
        return new Action
        {
            ActionType = Type.Move,
            DeltaMove = direction
        };
    }

    public static Action Tackle(Vector3 direction)
    {
        return new Action
        {
            ActionType = Type.Tackle,
            DeltaMove = direction
        };
    }

    public static Action Headbutt(Vector3 direction)
    {
        return new Action
        {
            ActionType = Type.Headbutt,
            ShootDirection = direction
        };
    }

    public static Action Dribble()
    {
        return new Action
        {
            ActionType = Type.Dribble
        };
    }

    public static Action Throw(Vector3 direction)
    {
        return new Action
        {
            ActionType = Type.Throw,
            ShootDirection = direction
        };
    }

    public static Action ChangePlayer()
    {
        return new Action
        {
            ActionType = Type.ChangePlayer

        };
    }
}
