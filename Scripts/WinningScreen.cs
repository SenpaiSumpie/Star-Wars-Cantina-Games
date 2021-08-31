using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningScreen : MonoBehaviour
{
    public void RestartLevel()
    {
        SceneManager.LoadScene("Pre-SoloSabacc");
    }
    public void QuitLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
