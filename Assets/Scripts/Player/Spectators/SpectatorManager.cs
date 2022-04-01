using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorManager : MonoBehaviour
{
    public GameObject[] Spectators;
    public bool isGoal = false;
    public int NombreCelebrationWithGoal = 2;
    public int NombreCelebrationWithOUTGoal = 1;


    void Start()
    {
         
    }

    void Update()
    {
        /*
        if (isGoal)
        {
            for (int i = 0; i < Spectators.Length; i++)
            {
                Spectators[i].GetComponent<Animator>().SetBool("SitVictory", true);
            }
        }
        else
        {
            for (int i = 0; i < Spectators.Length; i++)
            {
                Spectators[i].GetComponent<Animator>().SetBool("SitVictory", false);
            }
        }
        */
    }
}
