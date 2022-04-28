using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 1;

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
        _transition.SetTrigger("Start");

        yield return new WaitForSeconds(_transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
