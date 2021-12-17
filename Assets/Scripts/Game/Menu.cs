using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnInput(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Prototyping");
    }
}
