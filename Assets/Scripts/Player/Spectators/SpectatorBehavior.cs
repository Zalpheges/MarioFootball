using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorBehavior : MonoBehaviour
{
    public float ProbaCelebrationWithGoal =0.8f;
    public float ProbaCelebrationWithOUTGoal = 0.1f;
    public bool isSit;

    public SpectatorManager SpectatorManager;
    void Start()
    {
        SpectatorManager = gameObject.transform.parent.GetComponent<SpectatorManager>();
        gameObject.GetComponent<Animator>().SetBool("isSit", isSit);
    }

    void Update()
    {
        float random = Random.Range(0.0f, 1.0f);
        
        if (SpectatorManager.isGoal && random <= ProbaCelebrationWithGoal / 200)
        {
            gameObject.GetComponent<Animator>().SetBool("SitVictory", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("SitVictory", false);
        }
        
       
        
    }
}
