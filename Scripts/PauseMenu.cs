using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    bool isOn = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.GetComponent<CanvasGroup>().DOFade(0, 0.001f);
        pauseMenu.GetComponent<CanvasGroup>().interactable = false;
        pauseMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void UpdatePauseMenu()
    {
        if(isOn)
        {
            isOn = false;
            pauseMenu.GetComponent<CanvasGroup>().DOFade(0, 1f);
            pauseMenu.GetComponent<CanvasGroup>().interactable = false;
            pauseMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //pauseMenu.SetActive(false);
        }
        else
        {
            isOn = true;
            pauseMenu.GetComponent<CanvasGroup>().DOFade(1, 1f);
            pauseMenu.GetComponent<CanvasGroup>().interactable = true;
            pauseMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //pauseMenu.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Pre-SoloSabacc");
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
