using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorBehavior : MonoBehaviour
{
    public float ProbaCelebrationWithGoal =0.8f;
    public float ProbaCelebrationWithOUTGoal = 0.1f;

    public SpectatorManager SpectatorManager;
    void Start()
    {
        SpectatorManager = gameObject.transform.parent.GetComponent<SpectatorManager>();
    }

    void Update()
    {
        float random = Random.Range(0.0f, 1.0f);
        //Debug.Log(gameObject.name+" " +random);
        if (SpectatorManager.isGoal && random<= ProbaCelebrationWithGoal/200)
        {
            gameObject.GetComponent<Animator>().SetBool("SitVictory", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("SitVictory", false);
        }
    }
}
