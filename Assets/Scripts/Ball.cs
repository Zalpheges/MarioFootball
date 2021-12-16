using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 startingPoint;
    private Vector3 destination;
    private Vector3 bezierPoint;
    private float force;
    private bool trail;
    private bool isFree;
    private Color trailColorBegin;
    private Color trailColorEnd;

    private void Move()
    {
        
    }
    /// <summary>
    /// Dessine une traînée derrière la balle
    /// </summary>
    private void DrawTrail()
    {

    }
}
