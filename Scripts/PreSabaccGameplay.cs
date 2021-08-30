using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PreSabaccGameplay : MonoBehaviour
{
    // This is the deck for pre-sabacc deck, will utilize for later gameplay
    [SerializeField] Deck pre_sabacc_deck;
    [SerializeField] GameObject hand_location;

    // Deck size is utilized for a lot of gameplay functionality
    int deck_size;
    public bool discard_on = false;
    //[SerializeField] TextMeshProUGUI discard_text;

    // Keeps track of what card is on top of the playing deck
    [SerializeField] Image top_deck_image;
    int deck_top_value;
    Card top_of_deck;

    // Keeps track of what card is on top of the playing cards
    [SerializeField] Image top_discard_image;
    int discard_top_value;
    Card top_of_discard;

    // Discard pile containing cards (Not necessary?)
    Card[] discard_pile;
    public List<Card> card_hand;
    Card[] current_deck;

    [SerializeField] Image deck_vector3;
    [SerializeField] GameObject backgroundDeckImage;
    [SerializeField] Image discard_vector3;

    [SerializeField] GameObject card_discard_clip;
    [SerializeField] GameObject add_card_clip;
    [SerializeField] GameObject shuffleCardsClip;

    [SerializeField] GameObject cardToDiscard;
    [SerializeField] public Material outlineEffect;

    [SerializeField] Image playerAvatarImage;
    [SerializeField] TextMeshProUGUI playerUsername;

    public Player playerCharacter;

    //[SerializeField] Image left_image;
    //[SerializeField] Image right_image;

    //[SerializeField] TextMeshProUGUI current_hand_value;

    // Start is called before the first frame update
    void Start()
    {
        playerAvatarImage.sprite = GetComponent<PlayerProfile>().GetPlayerImage();
        playerUsername.text = GetComponent<PlayerProfile>().GetPlayerName();

        deck_size = pre_sabacc_deck.ReturnDeckSize();
        current_deck = new Card[deck_size];
        InitializeDeck();
        ShuffleDeck();
    }

    public void SetDiscard()
    {
        if (discard_on)
        {
            discard_on = false;
            //discard_text.text = "Discard: False";
        }
        else
        {
            discard_on = true;
            //discard_text.text = "Discard: True";
        }
    }

    // Update is called once per frame
    [Button("Deal Starting Hand")]
    public void Deal_Starting_Hand()
    {
        Deal_Card();
        Deal_Card();
    }

    [Button("Deal Card")]
    public void Deal_Card()
    {
        card_hand.Add(top_of_deck);

        top_deck_image.rectTransform.DOMove(new Vector2(hand_location.transform.position.x, hand_location.transform.position.y), 0.35f);

        add_card_clip.GetComponent<AudioSource>().Play();

        StartCoroutine(DealCardAnimation(0.35f));

        deck_top_value = deck_top_value - 1;
        top_of_deck = current_deck[deck_top_value];
        // set this to cardFace to see image of what card to be next
        top_deck_image.sprite = top_of_deck.cardBack;

        this.GetComponent<PlayerHand>().UpdateCurrentCardHand(playerCharacter);

        //top_deck_image.sprite = null;
    }

    IEnumerator DealCardAnimation(float seconds)
    {
        
        yield return new WaitForSeconds(seconds);
        top_deck_image.enabled = false;
        yield return new WaitForSeconds(seconds);
        top_deck_image.rectTransform.position = deck_vector3.transform.position;
        top_deck_image.enabled = true;
        //top_deck_image.rectTransform.DOMove(new Vector2(deck_vector3.transform.position.x, deck_vector3.transform.position.y), 0.01f);
    }

    IEnumerator DiscardCardAnimation(float seconds, int card_to_discard, Vector3 cachedDiscardLocation)
    {

        yield return new WaitForSeconds(seconds);
        // after waiting the given seconds, returns the card to its original location and sets it to false
        cardToDiscard.SetActive(false);
        cardToDiscard.transform.position = cachedDiscardLocation;
        //card_hand.RemoveAt(card_to_discard);
        //this.GetComponent<PlayerHand>().UpdateCurrentCardHand();
    }

    [Button("Discard Card")]
    public void Discard_Card(int card_to_discard)
    {
        // grab cardFace image so we can discard the card from the hand
        Sprite cached_sprite = card_hand[card_to_discard].cardFace;

        // grab the Vector 3 of the cardToDiscard
        Vector3 cachedDiscardLocation = new Vector3(cardToDiscard.transform.position.x, cardToDiscard.transform.position.y, cardToDiscard.transform.position.z);

        // set the top_of_discard equal to the new card
        top_of_discard = card_hand[card_to_discard];

        // removes the card from the hand
        card_hand.RemoveAt(card_to_discard);

        // updates the cards in the hand
        this.GetComponent<PlayerHand>().UpdateCurrentCardHand(playerCharacter);

        // sets the card to Discard image = to the cached sprite
        cardToDiscard.GetComponent<Image>().sprite = cached_sprite;

        // sets it to true so it can be seen moving
        cardToDiscard.SetActive(true);

        // moves the card to the vector2 of the discard pile
        cardToDiscard.transform.DOMove(new Vector2(discard_vector3.transform.position.x, discard_vector3.transform.position.y), 0.25f);

        // waits for the given time then sets the discarded card to false and resets its position
        StartCoroutine(DiscardCardAnimation(0.25f, card_to_discard, cachedDiscardLocation));

        // plays discard sound
        card_discard_clip.GetComponent<AudioSource>().Play();

        // sets the top of the discard pile to the new image
        top_discard_image.sprite = cached_sprite;

        // changes the discard value, i think this is obselete. ** CAN REMOVE**
        discard_top_value = discard_top_value + 1;
    }

    public void InitializeDeck()
    {
        int index;

        for( index = 0; index < deck_size; index++ )
        {
            current_deck[index] = pre_sabacc_deck.GetIndex(index);
        }
        // set this to cardFace to see which card
        top_deck_image.sprite = current_deck[deck_size - 1].cardBack;

        deck_top_value = deck_size - 1;

        top_of_deck = current_deck[deck_size - 1];
    }

    IEnumerator ResetDiscardPosition(float seconds, Vector3 cachedDiscardLocation)
    {
        yield return new WaitForSeconds(seconds);
        top_discard_image.enabled = true;

        top_discard_image.sprite = current_deck[deck_size - 1].cardBack;

        top_discard_image.transform.position = cachedDiscardLocation;
    }

    [Button("Shuffle Deck")]
    public void ShuffleDeck()
    {
        int randomized_value, index;
        int[] already_generated_values = new int[deck_size];
        Vector3 cachedDiscardLocation = new Vector3(top_discard_image.transform.position.x, top_discard_image.transform.position.y, top_discard_image.transform.position.z);
        Vector3 cachedHandLocation = new Vector3(cardToDiscard.transform.position.x, cardToDiscard.transform.position.y, cardToDiscard.transform.position.z);

        backgroundDeckImage.transform.DORotate(new Vector3(backgroundDeckImage.transform.rotation.x, backgroundDeckImage.transform.rotation.y, backgroundDeckImage.transform.rotation.z + 360), 1f, RotateMode.FastBeyond360).SetRelative(true);

        // display visual cue of shuffleing
        if (discard_top_value > 0)
        {
            // display discard card going from discard to deck
            

            top_discard_image.transform.DOMove(new Vector2(deck_vector3.transform.position.x, deck_vector3.transform.position.y), 0.45f);
            top_discard_image.enabled = false;
            StartCoroutine(ResetDiscardPosition(0.45f, cachedDiscardLocation));
        }
        if(card_hand.Count > 0)
        {
            // display card from hand going to deck location
            
            int card_to_discard = 0;

            // sets it to true so it can be seen moving
            cardToDiscard.SetActive(true);

            // moves the card to the vector2 of the discard pile
            cardToDiscard.transform.DOMove(new Vector2(deck_vector3.transform.position.x, deck_vector3.transform.position.y), 0.45f);

            // waits for the given time then sets the discarded card to false and resets its position
            StartCoroutine(DiscardCardAnimation(0.45f, card_to_discard, cachedHandLocation));
        }

        Card[] temp_deck = current_deck;
        discard_pile = new Card[deck_size];
        current_deck = new Card[deck_size];
        // Make sure hands are empty after shuffle
        card_hand.Clear();

        // play shuffle sound
        shuffleCardsClip.GetComponent<AudioSource>().Play();

        for (index = 0; index < deck_size; index++)
        {
            already_generated_values[index] = deck_size;
        }

        index = 0;
        while(index < deck_size)
        {
            // inclusive, exclusive 0, 10 = 0-9
            randomized_value = Random.Range(0, deck_size);
            if( already_generated_values.Contains<int>(randomized_value))
            {
                Debug.Log("The generated value has already been generated");
            }
            else
            {
                Debug.Log("The generated value is new!");
                already_generated_values[index] = randomized_value;
                current_deck[index] = temp_deck[randomized_value];
                index++;
            }
        }
        // set this to cardFace to see what card you're getting
        top_deck_image.sprite = current_deck[deck_size - 1].cardBack;
        deck_top_value = deck_size - 1;
        top_of_deck = current_deck[deck_size - 1];

        this.GetComponent<PlayerHand>().UpdateCurrentCardHand(playerCharacter);
    }
}
