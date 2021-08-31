using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GamePot : MonoBehaviour
{
    [SerializeField] public string potName;                        // name of the credit game pot
    [SerializeField] public int potTotal;                          // amount of credits in the game pot
    [SerializeField] TextMeshProUGUI nameOfPot;                    // game object for displaying the game pot name
    [SerializeField] public TextMeshProUGUI creditTotal;           // game object for displaying the game pot credit total

    // Start is called before the first frame update
    void Start()
    {
        // set the text to the pot's name
        nameOfPot.text = potName;
    }

    void Update()
    {
        // if potTotal is greater or equal to zero then it updates its credit total
        if(potTotal >= 0)
        {
            creditTotal.text = potTotal.ToString();
        }
    }

    /*
     * adds amount given to the pot total
     */
    public void AddToPot(int totalToAdd)
    {
        potTotal = potTotal + totalToAdd;
    }

    /*
     * Removes contents of pot total and returns them for
     * the player to 'win'
     */
    public int WonPot()
    {
        int tempPotTotal = potTotal;
        potTotal = 0;
        return tempPotTotal;
    }
}
