using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "data", menuName = "Card")]
public class Card : ScriptableObject
{
    // This is everything a 'Card' object needs to contain
    [SerializeField] Sprite cardBack;                           // The back of the card
    [SerializeField] Sprite cardFace;                           // the face image of the card
    [SerializeField] int cardValue;                             // total value of the card
    [SerializeField] string cardName;                           // name of the card
    [SerializeField] string cardSuit;                           // which suit the card belongs to, not used

    // returns the cardBack sprite
    public Sprite ReturnCardBack()
    {
        return cardBack;
    }

    // returns the cardFace sprite
    public Sprite ReturnCardFace()
    {
        return cardFace;
    }

    // returns the cardValue int
    public int ReturnCardValue()
    {
        return cardValue;
    }

    // returns the cardName string
    public string ReturnCardName()
    {
        return cardName;
    }

    // returns the cardSuit string, *NOT USED*
    public string ReturnCardSuit()
    {
        return cardSuit;
    }    
}
