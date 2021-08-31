using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data", menuName = "Deck")]
public class Deck : ScriptableObject
{
    // a deck consists of an array of card objects
    [SerializeField] Card[] currentDeck;

    // returns a card at specific index
    public Card GetIndex(int value)
    {
        return currentDeck[value];
    }

    // gets the total size of the deck
    public int ReturnDeckSize()
    {
        return currentDeck.Length;
    }    
}

