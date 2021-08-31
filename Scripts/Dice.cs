using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Dice : MonoBehaviour
{
    // in game dice game objects
    [SerializeField] GameObject diceNumberOne;
    [SerializeField] GameObject diceNumberTwo;

    // dice images (sides of the dice)
    [SerializeField] Sprite diceFaceOne;
    [SerializeField] Sprite diceFaceTwo;
    [SerializeField] Sprite diceFaceThree;
    [SerializeField] Sprite diceFaceFour;
    [SerializeField] Sprite diceFaceFive;
    [SerializeField] Sprite diceFaceSix;

    // sabacc shift information
    [SerializeField] TextMeshProUGUI sabaccShiftText;
    public bool sabaccShift = false;

    // dice roll sound
    [SerializeField] GameObject diceRollSound;

    private void Start()
    {
        sabaccShiftText.alpha = 0;                  // Sabacc shift will be 1 if there is a sabacc shift
    }

    /*
     * Simulates rolling two dice and displays the outcome
     */
    [Button("Roll Dice")]
    public void RollDice()
    {
        // declaration of local variables
        int firstDiceRoll, secondDiceRoll;

        // remove any previous 'sabacc shifts', shouldnt really apply as the 'Dice Round' will remove the whole UI
        // just a precaution
        sabaccShift = false;
        sabaccShiftText.alpha = 0;

        // play dice roll sound
        diceRollSound.GetComponent<AudioSource>().Play();

        // roll both dice, with different values
        firstDiceRoll = Random.Range(1, 7);
        secondDiceRoll = Random.Range(1, 7);

        // set the dice to the corresponding dice face
        DisplayCorrectImage(firstDiceRoll, diceNumberOne);
        DisplayCorrectImage(secondDiceRoll, diceNumberTwo);

        // check for sabacc shift, dice1 == dice2
        if ( firstDiceRoll == secondDiceRoll )
        {
            sabaccShift = true;
            sabaccShiftText.DOFade(1, 1f);
        }
    }

    /*
     * Displays the correct 'face' of the dice roll
     */
    void DisplayCorrectImage(int valueRolled, GameObject whichDice)
    {
        // sets the given game object to the correct dice face
        if (valueRolled == 1)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceOne;
        }
        else if (valueRolled == 2)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceTwo;
        }
        else if (valueRolled == 3)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceThree;
        }
        else if (valueRolled == 4)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceFour;
        }
        else if (valueRolled == 5)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceFive;
        }
        else if (valueRolled == 6)
        {
            whichDice.GetComponent<Image>().sprite = diceFaceSix;
        }
        // not any of the images, debug.log
        else
        {
            Debug.LogError("Invalid Roll");
        }
    }
}
