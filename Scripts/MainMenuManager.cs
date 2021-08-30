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
    // all menu canvas groups
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
        mainMenu.DOFade(1, 3f);
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        // sets the initial sprite and text of the profile stuff. 
        playerAvatarImage.sprite = GetComponent<PlayerProfile>().availablePortraits[0];
        playerChosenName.text = GetComponent<PlayerProfile>().GetPlayerName();
    }

    public void DeactivateCanvasGroup(CanvasGroup givenCanvas)
    {
        givenCanvas.DOFade(0, 1f);
        givenCanvas.interactable = false;
        givenCanvas.blocksRaycasts = false;
    }

    public void ActivateCanvasGroup(CanvasGroup givenCanvas)
    {
        givenCanvas.DOFade(1, 1f);
        givenCanvas.interactable = true;
        givenCanvas.blocksRaycasts = true;
    }

    /*
     * Main Menu Functions
     */
    public void ToSingleplayerFromMainMenu()
    {
        DeactivateCanvasGroup(mainMenu);
        ActivateCanvasGroup(singleplayerMenu);
    }

    public void ToMultiplayerFromMainMenu()
    {
        DeactivateCanvasGroup(mainMenu);
        ActivateCanvasGroup(multiplayerMenu);
    }

    public void ToProfileFromMainMenu()
    {
        DeactivateCanvasGroup(mainMenu);
        ActivateCanvasGroup(profileMenu);
    }

    public void ToOptionsFromMainMenu()
    {
        DeactivateCanvasGroup(mainMenu);
        ActivateCanvasGroup(optionsMenu);
    }

    public void QuitMainMenu()
    {
        Application.Quit();
    }

    /*
     * Singleplayer Functions
     */
    public void ToMainMenuFromSingleplayer()
    {
        DeactivateCanvasGroup(singleplayerMenu);
        ActivateCanvasGroup(mainMenu);
    }

    public void ToPreSoloSabaccFromSingleplayer()
    {
        DeactivateCanvasGroup(singleplayerMenu);
        ActivateCanvasGroup(preSoloSabaccMenu);
    }

    /*
     * Pre-Solo Sabacc functions
     */
    public void StartPreSoloSabacc()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadHowToPlayPreSoloSabacc()
    {
        DeactivateCanvasGroup(preSoloSabaccMenu);
        ActivateCanvasGroup(preSoloSabaccHowToPlayMenu);
    }

    public void BackToPreSoloSabaccFromHowToPlay()
    {
        DeactivateCanvasGroup(preSoloSabaccHowToPlayMenu);
        ActivateCanvasGroup(preSoloSabaccMenu);
    }

    public void ToSinglePlayerFromPreSoloSabacc()
    {
        DeactivateCanvasGroup(preSoloSabaccMenu);
        ActivateCanvasGroup(singleplayerMenu);
    }

    /*
     * Multiplayer Functions
     */
    public void ToMainMenuFromMultiplayer()
    {
        DeactivateCanvasGroup(multiplayerMenu);
        ActivateCanvasGroup(mainMenu);
    }

    /*
     * Profile Functions
     */
    public void ToMainMenuFromProfile()
    {
        DeactivateCanvasGroup(profileMenu);
        ActivateCanvasGroup(mainMenu);
    }

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
