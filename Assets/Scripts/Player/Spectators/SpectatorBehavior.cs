using UnityEngine;

public class SpectatorBehavior : MonoBehaviour
{
    public float ProbaCelebrationWithGoal = 0.8f;
    private bool _isSit = true;
    void Start()
    {
        gameObject.GetComponent<Animator>().SetBool("isSit", _isSit);
    }

    void Update()
    {
        float random = Random.Range(0.0f, 1.0f);

        if (GameManager.IsGoalScored)
        {
            if (random <= ProbaCelebrationWithGoal)
                gameObject.GetComponent<Animator>().SetBool("SitVictory", true);
        }
        else
            gameObject.GetComponent<Animator>().SetBool("SitVictory", false);
    }
}
