using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

/*
 * Not currently in use, was used in previous edition, would like to use
 * instead of keep track in each player script? 
 */
public class Credits : MonoBehaviour
{
    // Player information...
    [SerializeField] string playerName;                                     // which player is being referenced
    [SerializeField] public int totalPlayerCredits;                         // players current credits
    [SerializeField] public int selectedCreditAmount;                       // this was used for a slider
    [SerializeField] public int betTotal;                                   // current betting total

    // For animations...
    [SerializeField] GamePot chipPot;                                       // where the credit image travels too
    [SerializeField] TextMeshProUGUI creditTotal;                           // where the credit image travels from
    [SerializeField] GameObject creditImage;                                // the credit image used for animation

    private void Start()
    {
        selectedCreditAmount = betTotal;                                    // initializes credit to current bet total
    }

    private void Update()
    {
        creditTotal.text = totalPlayerCredits.ToString();                   // displays players current credit total
    }

    /*
     * Reduces the players credit total by the amount given
     * if reduces below zero it sets total to zero
     */
    public void ReducePlayerCredits(int creditReduction)
    {
        totalPlayerCredits = totalPlayerCredits - creditReduction;

        if(totalPlayerCredits < 0)
        {
            totalPlayerCredits = 0;
        }
    }

    /*
     * BetChips plays the starting portion of the animation for the credit image
     */
    public void BetChips()
    {
        creditImage.transform.position = new Vector3(creditTotal.rectTransform.position.x, creditTotal.rectTransform.position.y, creditTotal.rectTransform.position.z);         // resets creditImage
        creditImage.SetActive(true);                                                                                                                                            // turns on creditImage
        creditImage.transform.DOMove(new Vector2(chipPot.creditTotal.rectTransform.position.x, chipPot.creditTotal.rectTransform.position.y), 0.35f);                         // moves credit image
        StartCoroutine(ChipMoveToPileAnimation(0.35f));                                                                                                                         // starts coroutine
    }

    /*
     * Starts a coroutine to wait to turn the creditImage back on
     */
    IEnumerator ChipMoveToPileAnimation(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        creditImage.SetActive(false);
    }
}
