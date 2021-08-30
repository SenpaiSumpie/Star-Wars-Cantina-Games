using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "Deck")]
public class Deck : ScriptableObject
{
    [SerializeField] Card[] currentDeck;

    public Card GetTopCard()
    {
        return currentDeck[currentDeck.Length];
    }

    public Card GetIndex(int value)
    {
        return currentDeck[value];
    }
    public int ReturnDeckSize()
    {
        return currentDeck.Length;
    }    
}

