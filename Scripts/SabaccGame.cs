
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * First iteration of MAINGAMEPLAYSABACC.cs <- this is the current game
 */
public class SabaccGame : MonoBehaviour
{
    [SerializeField] Deck receivedDeck;

    int deckSize;
    int placeHolderDeckTopValue;
    int placeHolderDiscardTopValue;
    //int discardPileSize = 0;

    Card[] discardPile;
    Card[] cardHand = new Card[2];
    Card[] currentDeck;

    Card topOfDiscard;
    Card topOfDeck;

    [SerializeField] Image topDeckImage;
    [SerializeField] Image topDiscardImage;

    [SerializeField] Image leftCard;
    [SerializeField] Image rightCard;

    [SerializeField] Text currentValueTotal;

    // Start is called before the first frame update
    void Start()
    {
        deckSize = receivedDeck.ReturnDeckSize();
        currentDeck = new Card[deckSize];
        InitializeDeck();
        placeHolderDeckTopValue = deckSize - 1;
        ShuffleDeck();
        //topOfDeck = currentDeck[placeHolderDeckTopValue];
        DealTwoCards();
    }

    void Update()
    {
        NoMoreCardsInDeck();
        
        UpdateCurrentDeck();
    }

    public void AddCardToHand()
    {
        if(leftCard.sprite == null)
        {
            leftCard.sprite = topOfDeck.ReturnCardFace();
            placeHolderDeckTopValue--;
            topOfDeck = currentDeck[placeHolderDeckTopValue];
        }   
        else if( rightCard.sprite == null)
        {
            rightCard.sprite = topOfDeck.ReturnCardFace();
            placeHolderDeckTopValue--;
            topOfDeck = currentDeck[placeHolderDeckTopValue];
        }
        else
        {
            Debug.Log("The hand is full!");
        }
    }

    public void UpdateDiscardPile()
    {
        topOfDiscard = discardPile[placeHolderDiscardTopValue];
        topDiscardImage.sprite = topOfDiscard.ReturnCardFace();
    }

    public void UpdateCurrentDeck()
    {
        Debug.Log("CURRENT PLACE HOLDER VALUE" + placeHolderDeckTopValue);
        topDeckImage.sprite = currentDeck[placeHolderDeckTopValue].ReturnCardFace();
    }

    public void NoMoreCardsInDeck()
    {
        //Checks to see if the current deck ever has no more cards
        if(placeHolderDeckTopValue < 0)
        {
            //If it does it calls for a shuffle deck
            ShuffleDeck();
        }
    }

    public void DealTwoCards()
    {
        leftCard.sprite = topOfDeck.ReturnCardFace();

        placeHolderDeckTopValue--;

        topOfDeck = currentDeck[placeHolderDeckTopValue];

        rightCard.sprite = topOfDeck.ReturnCardFace();

        placeHolderDeckTopValue--;

        topOfDeck = currentDeck[placeHolderDeckTopValue];
    }

    public void DiscardLeftCard()
    {
        placeHolderDeckTopValue--;
        leftCard.enabled = false;
        leftCard.sprite = null;
        UpdateDiscardPile();
    }

    public void DiscardRightCard()
    {
        placeHolderDiscardTopValue++;
        placeHolderDeckTopValue--;
        rightCard.sprite = null;
        UpdateDiscardPile();
    }

    public void InitializeDeck()
    {
        int index;//, generatedValue;

        for( index = 0; index < deckSize; index++ )
        {
            currentDeck[index] = receivedDeck.GetIndex(index);//should be generatedValue
        }

        topOfDeck = currentDeck[deckSize - 1];
    }

    public void ShuffleDeck()
    {
        int generatedValue, index;
        int[] generatedValues = new int[deckSize];

        for(index = 0; index < deckSize; index++)
        {
            generatedValues[index] = deckSize;
        }

        Card[] tempDeck = currentDeck;
        
        //Will create a new random deck from the previous deck
        currentDeck = new Card[deckSize];
        //Resets Discard Pile
        discardPile = new Card[deckSize];
        //Resets the currentDeck
        placeHolderDeckTopValue = deckSize - 1;

        //discardPileSize = 0;
        index = 0;
        while(index < deckSize)
        {
            generatedValue = Random.Range(0, deckSize - 1);
            
            if( generatedValues.Contains<int>(generatedValue))
            {
                Debug.Log("The generated value has already been generated");
            }
            else
            {
                Debug.Log("The generated value is new!");
                generatedValues[index] = generatedValue;
                currentDeck[index] = tempDeck[generatedValue];
                index++;
            }
            index++;
        }
        topOfDeck = currentDeck[deckSize - 1];
    }
}
