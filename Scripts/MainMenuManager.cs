using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class MainMenuManager : MonoBehaviour
{
    // all menu canvas groups, not necessary but keeping for now
    [Title("Menu Canvas Groups")]
    [SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup singleplayerMenu;
    [SerializeField] CanvasGroup multiplayerMenu;
    [SerializeField] CanvasGroup profileMenu;
    [SerializeField] CanvasGroup optionsMenu;
    [SerializeField] CanvasGroup preSoloSabaccMenu;
    [SerializeField] CanvasGroup preSoloSabaccHowToPlayMenu;

    // player information
    [Title("Player Information")]
    [SerializeField] Image playerAvatarImage;
    [SerializeField] TextMeshProUGUI playerChosenName;

    // Start is called before the first frame update
    void Start()
    {
        // main menu's initial load in is different
        mainMenu.DOFade(1, 1.5f);
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        // sets the initial sprite and text of the profile stuff. 
        playerAvatarImage.sprite = GetComponent<PlayerProfile>().availablePortraits[0];
        playerChosenName.text = GetComponent<PlayerProfile>().GetPlayerName();
    }

    public void DeactivateCanvasGroup(CanvasGroup givenCanvas)
    {
        givenCanvas.DOFade(0, 0.75f);
        givenCanvas.interactable = false;
        givenCanvas.blocksRaycasts = false;
    }

    public void ActivateCanvasGroup(CanvasGroup givenCanvas)
    {
        givenCanvas.DOFade(1, 0.75f);
        givenCanvas.interactable = true;
        givenCanvas.blocksRaycasts = true;
    }

    /*
     * Main Menu functions
     */
    public void QuitMainMenu()
    {
        Application.Quit();
    }

    /*
     * Pre-Solo Sabacc functions
     */
    public void StartPreSoloSabacc()
    {
        SceneManager.LoadScene(3);
    }

    /*
     * Profile Functions
     */
    public void SetPlayerName(string newPlayerName)
    {
        GetComponent<PlayerProfile>().SetPlayerName(newPlayerName);
    }

    public void SetPlayerImageRight()
    {
        GetComponent<PlayerProfile>().SetPlayerImageRight();
        playerAvatarImage.sprite = GetComponent<PlayerProfile>().GetPlayerImage();
    }

    public void SetPlayerImageLeft()
    {
        GetComponent<PlayerProfile>().SetPlayerImageLeft();
        playerAvatarImage.sprite = GetComponent<PlayerProfile>().GetPlayerImage();
    }
}
