using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour
{
    // player information, Name and Image, potentially total credits would be located here too.
    public static string playerName;
    public static Sprite playerImage;

    [SerializeField] public Sprite[] availablePortraits;
    public static int portraitNumber;

    [SerializeField] Player player;

    private void Start()
    {
        if(playerImage == null && playerName == null)
        {
            playerImage = availablePortraits[0];
            playerName = "Default Username";
        }
        if (player != null)
        {
            player.thisPlayerAvatar = playerImage;
            player.thisPlayerName = playerName;
        }
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public Sprite GetPlayerImage()
    {
        return playerImage;
    }

    public void SetPlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        Debug.Log(playerName);
    }

    public void SetPlayerImageLeft()
    {
        if(availablePortraits.Length - 1 >= 0)
        {
            portraitNumber = portraitNumber - 1;
        }
        if(portraitNumber < 0)
        {
            portraitNumber = availablePortraits.Length - 1;
        }
        playerImage = availablePortraits[portraitNumber];
        Debug.Log("Index" + portraitNumber);

    }

    public void SetPlayerImageRight()
    {
        if(availablePortraits.Length - 1 >= 0)
        {
            portraitNumber = portraitNumber + 1;
        }
        
        if(portraitNumber > availablePortraits.Length - 1)
        {
            portraitNumber = 0;
        }

        playerImage = availablePortraits[portraitNumber];
        Debug.Log("Index" + portraitNumber);
    }
}
