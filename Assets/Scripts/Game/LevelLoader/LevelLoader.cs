using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1;

    private static LevelLoader _instance;
    private void Awake()
    {
        _instance = this;
    }
    public static void LoadNextLevel(int levelIndex)
    {
        _instance.StartCoroutine(_instance.LoadLevel(levelIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
