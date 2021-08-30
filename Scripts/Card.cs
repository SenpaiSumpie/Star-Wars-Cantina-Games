using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "data", menuName = "Card")]
public class Card : ScriptableObject
{
    [SerializeField] public Sprite cardBack;
    [SerializeField] public Sprite cardFace;
    [SerializeField] public int cardValue;
    [SerializeField] public string cardName;
    [SerializeField] public string cardSuit;

    public Sprite ReturnCardBack()
    {
        return cardBack;
    }

    public Sprite ReturnCardFace()
    {
        return cardFace;
    }

    public int ReturnCardValue()
    {
        return cardValue;
    }

    public string ReturnCardName()
    {
        return cardName;
    }

    public string ReturnCardSuit()
    {
        return cardSuit;
    }    
}
