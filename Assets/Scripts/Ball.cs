using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 _startingPoint;
    private Vector3 _destination;
    private Vector3 _bezierPoint;
    private float _force;
    private bool _trail;
    private bool _isFree;
    private Color _trailColorBegin;
    private Color _trailColorEnd;

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
