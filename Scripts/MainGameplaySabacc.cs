using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;

public class MainGameplaySabacc : MonoBehaviour
{
    // get the game deck
    [Title("Game Deck: Pre Solo Sabacc Deck")]
    [SerializeField] Deck preSabaccDeck;

    // deck size for use in many methods, discardOn used for being able to discard a card
    int deckSize;

    // keeps track of the playing deck
    [SerializeField] Image topDeckImage;
    int deckTopValue;
    Card topOfDeck;

    // keeps track of the discard deck
    [SerializeField] Image topDiscardImage;
    int discardTopValue;
    Card topOfDiscard;

    // discard, player hand, and current deck
    Card[] discardPile; // may not be necessary
    Card[] currentDeck;

    // game states for use in determining what occurs
    // may not be necessary
    enum sabaccGameStates
    {
        PLAYING,
        FINISHED
    };
    sabaccGameStates gameStates = sabaccGameStates.PLAYING;

    // used to loop through the number of players for every turn
    [Title("Players")]
    [SerializeField] public List<Player> players;

    // keeps track of vector3s, of the decks
    [Title("Deck Locations for Use in Animations")]
    [SerializeField] Image deckVector3;
    [SerializeField] GameObject backgroundDeckImage;
    [SerializeField] Image discardVector3;

    // Audio clips for use throughout code
    [Title("Audio Clips")]
    [SerializeField] GameObject cardDiscardClip;
    [SerializeField] GameObject addCardClip;
    [SerializeField] GameObject shuffleCardsClip;
    [SerializeField] GameObject cardFlipClip;
    [SerializeField] GameObject uiPrompt;
    [SerializeField] GameObject uiExit;

    // GameObject to simulate getting discarded
    [Title("Discarded Card")]
    [SerializeField] GameObject cardToDiscard;

    // Material to show outline around card
    [Title("Materials")]
    [SerializeField] public Material outlineEffect;

    // Player avatar and name
    [Title("Player Information")]
    [SerializeField] Image playerAvaterImage;
    [SerializeField] TextMeshProUGUI playerUsername;
    [SerializeField] PlayerProfile playerCharacter;

    // Game pots for betting into
    [Title("Game Pots")]
    [SerializeField] GamePot sabaccPot;
    [SerializeField] GamePot handPot;

    // bools for keeping track of various stages of the game
    bool buttonPressed = false;
    bool pickedCardToDiscard = false;
    public bool discardOn = false;

    // All of the Interactabble menues
    [Title("Interactable Menus")]
    [SerializeField] GameObject bettingMenu;
    [SerializeField] GameObject tradingRound;
    [SerializeField] GameObject diceRound;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI playerWinsText;

    // dice script in the gameplay menu
    [Title("Dice Script")]
    [SerializeField] Dice sabaccDice;

    // how many credits should each player start with?
    [Title("Starting Credits")]
    [SerializeField] int startingCredits = 2000;

    // objects for credit animations
    [Title("Credit Animation Items")]
    [SerializeField] GameObject creditImage;
    [SerializeField] GameObject creditExchangeClip;
    [SerializeField] GameObject creditWinClip;

    // the game information text, "whats going on from stage to stage"
    [Title("Game Informer")]
    [SerializeField] TextMeshProUGUI gameInformation;

    // information to help the player make better decisions
    [Title("Betting Round Text for Player")]
    [SerializeField] TextMeshProUGUI callValueText;
    [SerializeField] TextMeshProUGUI raiseValueText;
    [SerializeField] TextMeshProUGUI allInValueText;

    /*
     * FUNCTION LIST:
     * AllInChoice
     * BetAnimation
     * BombOut
     * ButtonPressed
     * CallChoice
     * DealCard
     * DealCardPart2
     * DiscardCard
     * DiscardCardAnimation
     * DisplayGameWinningScreen
     * DisplayOptionsForRound
     * GetBestAIBetOption
     * GetBetAITradeOption
     * HandPotAnimation
     * HidePlayerHand
     * InitializeDeck
     * MoveOntoNextPlayer
     * PayBlind
     * PaySabaccPayment
     * PickedCard
     * RaiseChoice
     * RemoveOptionsForRound
     * RevealPlayerHand
     * RollDice
     * SabaccPotAnimation
     * SetPlayersCreditTotal()
     * ShuffleDeck
     * Start
     * StartGame
     * UpdateTotalAmountBetted
     */
    
    /*
     * If a player chooses all in then they bet all their chips
     */
    private void AllInChoice(Player player)
    {
        // bet all the players chips
        player.BetChips(player.credits, handPot);

        // Updates players credit total better
        UpdateTotalAmountBetted(player, player.credits);
    }

    /*
     * Displays the bet animation, previously in Credits.cs
     */
    private void BetAnimation(Player player, GamePot potToBetTo)
    {
        // play audio clip of betting
        creditExchangeClip.GetComponent<AudioSource>().Play();
        
        // moves credit image to players credit total amount location
        creditImage.transform.position = new Vector3(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y, player.GetCreditText().rectTransform.position.z);

        // turns on the credit image
        creditImage.SetActive(true);
        
        // tweens the credit image to the new pot location
        creditImage.transform.DOMove(new Vector2(potToBetTo.creditTotal.rectTransform.position.x, potToBetTo.creditTotal.rectTransform.position.y), 0.35f);
    }

    /*
     * Player has bombed out so they must pay 10% of winning pot to the sabacc pot
     */
    private void BombOut(Player player)
    {
        // determines 10% of winning pot
        int bombOutAmount = handPot.potTotal / 10;

        // checks to see if bombing out is more than the player has
        if (bombOutAmount > player.credits)
        {
            bombOutAmount = player.credits;
        }

        // pay the 10% to the sabacc pot
        player.BetChips(bombOutAmount, sabaccPot);
    }

    // Button pressed is used for buttons in game to call and set button pressed to true
    public void ButtonPressed()
    {
        buttonPressed = true;
    }

    /*
     * Player has called the current betting amount
     */
    private void CallChoice(Player player, int currentAmountToBet)
    {
        // determines the amount of credits to bet
        int callValue = currentAmountToBet - player.amountBettedThisRound;
        
        // bets the total amount
        player.BetChips(callValue, handPot);

        // updates the player total credit value
        UpdateTotalAmountBetted(player, callValue);
    }

    // deals card to specific player
    private void DealCard(Player player)
    {
        // adds a card to the specific player hand
        player.cardHand.Add(topOfDeck);

        // plays the animation of the dealing card
        topDeckImage.rectTransform.DOMove(new Vector2(player.handLocation.transform.position.x, player.handLocation.transform.position.y), 0.35f);

        // plays sound of dealing card
        addCardClip.GetComponent<AudioSource>().Play();
    }

    /*
     * finishes dealing card to the player,
     * this is in two parts so i can WaitForSeconds in between
     */
    private void DealCardPart2(Player player)
    {
        // from DealCardAnimation
        topDeckImage.rectTransform.position = deckVector3.transform.position;
        topDeckImage.enabled = true;

        // sets new top of deck value
        deckTopValue = deckTopValue - 1;
        topOfDeck = currentDeck[deckTopValue];

        // set this to cardFace to see image of what card to be next
        topDeckImage.sprite = topOfDeck.ReturnCardBack();

        // updates players hand with new card
        player.UpdateCurrentCardHand();
    }

    /*
     * discards a card from the player
     */
    public void DiscardCard(Player player, int cardIndexToDiscard)
    {
        // grab cardFace image so we can discard the card from the hand
        Sprite cachedSprite = player.cardHand[cardIndexToDiscard].ReturnCardFace();

        // grab the Vector 3 of the cardToDiscard
        Vector3 cachedDiscardLocation = new Vector3(player.handLocation.transform.position.x, player.handLocation.transform.position.y, player.handLocation.transform.position.z);
        cardToDiscard.transform.position = new Vector3(player.handLocation.transform.position.x, player.handLocation.transform.position.y, player.handLocation.transform.position.z);

        // set the top_of_discard equal to the new card
        topOfDiscard = player.cardHand[cardIndexToDiscard];

        // removes the card from the hand
        player.cardHand.RemoveAt(cardIndexToDiscard);

        // updates the cards in the hand
        player.UpdateCurrentCardHand();

        // sets the card to Discard image = to the cached sprite
        cardToDiscard.GetComponent<Image>().sprite = cachedSprite;

        // sets it to true so it can be seen moving
        cardToDiscard.SetActive(true);

        // moves the card to the vector2 of the discard pile
        cardToDiscard.transform.DOMove(new Vector2(discardVector3.transform.position.x, discardVector3.transform.position.y), 0.25f);

        // waits for the given time then sets the discarded card to false and resets its position
        StartCoroutine(DiscardCardAnimation(0.25f, cardIndexToDiscard, cachedDiscardLocation));

        // plays discard sound
        cardDiscardClip.GetComponent<AudioSource>().Play();

        // sets the top of the discard pile to the new image
        topDiscardImage.sprite = cachedSprite;

        // changes the discard value, i think this is obselete. ** CAN REMOVE**
        discardTopValue++;
    }

    /*
     * Plays the discard card animation, properly Ienumerator would be in StartGame
     */
    IEnumerator DiscardCardAnimation(float seconds, int cardIndexToDiscard, Vector3 cachedDiscardLocation)
    {
        // waits for designated amount of time
        yield return new WaitForSeconds(seconds);

        // sets card to not active and restores its discard position
        cardToDiscard.SetActive(false);
        cardToDiscard.transform.position = cachedDiscardLocation;
    }

    /*
     * Displays the end of the game
     */
    private void DisplayGameWinningScreen(Player player)
    {
        // if the player has a vocal queue play the victory chant
        if (player.hasVocalQueues)
        {
            player.PlayVictory();
        }

        playerWinsText.text = player.thisPlayerName + " wins!";                     // state who has won
        gameOverScreen.GetComponent<CanvasGroup>().DOFade(1, 1f);                   // fades the menu in
        gameOverScreen.GetComponent<CanvasGroup>().interactable = true;             // sets it to interactable
        gameOverScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;           // blocks raycasts
    }

    // Displays the various game menus 
    private void DisplayOptionsForRound(GameObject menu)
    {
        uiPrompt.GetComponent<AudioSource>().Play();                            // plays audio source for displaying menu
        menu.GetComponent<CanvasGroup>().DOFade(1, 1f);                         // fades the menu in
        menu.GetComponent<CanvasGroup>().interactable = true;                   // makes the menu interactable
        menu.GetComponent<CanvasGroup>().blocksRaycasts = true;                 // blocks raycasts
    }

    /*
     * Function to determine what the AI should pick for betting
     * not the best function, just a quick easy solution IMO
     */
    private void GetBestAIBetOption(Player player, int currentAmountToBet)
    {
        // gets a random value to simulate various player choices
        int random = Random.Range(0, player.distanceFrom23 + 4);
        int callValue = currentAmountToBet - player.amountBettedThisRound;

        // if the player has a hand greater than 23away from the goal then they should FOLD
        if (player.distanceFrom23 >= 23)
        {
            player.betChoice = Player.BettingChoice.FOLD;
        }
        // if the player is 1-5 from the goal and random is the same value then RAISE
        else if (random == player.distanceFrom23 && player.distanceFrom23 >= 1 && player.distanceFrom23 <= 5)
        {
            player.betChoice = Player.BettingChoice.RAISE;
        }
        // if the player has 23 exactly then ALL IN, random also needs to equal 0
        else if (player.distanceFrom23 == 0 && random == 0)
        {
            player.betChoice = Player.BettingChoice.ALLIN;
        }
        else
        {
            // if the call amount is 0 then player will CHECK
            if (callValue == 0)
            {
                player.betChoice = Player.BettingChoice.CHECK;
            }
            // otherwise they will CALL
            else
            {
                player.betChoice = Player.BettingChoice.CALL;
            }
        }
    }

    /*
     * Function to determine what the AI should pick for trading
     * not the best function, just a quick easy solution IMO
     */
    private void GetBestAITradeOption(Player player)
    {
        // gets a random value for simulating player choice
        int random = Random.Range(1, 10);

        // if the players hand needs work it TRADES a card
        if (player.distanceFrom23 > 15)
        {
            player.tradeChoice = Player.TradingChoice.TRADE;
        }
        // if its close but not quite it ADDS a card
        else if (player.distanceFrom23 > 7)
        {
            player.tradeChoice = Player.TradingChoice.ADD;
        }
        // if its really close it STANDS
        else if (player.distanceFrom23 > 2)
        {
            player.tradeChoice = Player.TradingChoice.STAND;
        }
        else
        {
            // STANDS if close to 23 and random is less than 5 50% chance
            if (random < 5)
            {
                player.tradeChoice = Player.TradingChoice.STAND;
            }
            // calls the game otherwise
            else
            {
                player.tradeChoice = Player.TradingChoice.ALDERAAN;
            }
        }
    }

    // Shows Hand pot animation Player winning a round
    private void HandPotAnimation(Player player, GamePot potToBetTo)
    {
        // plays audio source
        creditWinClip.GetComponent<AudioSource>().Play();

        // sets credit images position
        creditImage.transform.position = new Vector3(potToBetTo.creditTotal.rectTransform.position.x, potToBetTo.creditTotal.rectTransform.position.y, potToBetTo.creditTotal.rectTransform.position.z);
        
        // sets credits image to true
        creditImage.SetActive(true);

        // moves the credit image
        creditImage.transform.DOMove(new Vector2(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y), 0.35f);
    }

    /*
     * hides the given players hand
     */
    private void HidePlayerHand(Player player)
    {
        // sets the bool saying the cards aren't revealed
        player.revealCards = false;

        // updates player hand cards
        player.UpdateCurrentCardHand();
        
        // plays audio source
        cardFlipClip.GetComponent<AudioSource>().Play();
    }

    /*
     * Sets the generated card deck equal to the predetermined sabacc deck
     */
    public void InitializeDeck()
    {
        // temp index
        int index;

        for (index = 0; index < deckSize; index++)
        {
            // assigns every currentDeck index = to the preSabaccDeck
            currentDeck[index] = preSabaccDeck.GetIndex(index);
        }

        // set to cardFace to see if what the top card is
        topDeckImage.sprite = currentDeck[deckSize - 1].ReturnCardBack();
        deckTopValue = deckSize - 1;
        topOfDeck = currentDeck[deckSize - 1];
    }

    // Fancy function to determine who the next player in line is
    private int MoveOntoNextPlayer(int player)
    {
        // sets nextPlayer equal to the next player
        int nextPlayer = player + 1;

        // if the nextPlayer is not in the players it resets to the first player
        if (nextPlayer >= players.Count)
        {
            nextPlayer = 0;
        }
        return nextPlayer;
    }

    /*
     * Pay blind payment at beginning of round
     */
    private void PayBlind(Player player, int blindPayment)
    {
        // move the credit chip
        // still need to add the animation
        // reduce player chip count and add it to the pot
        player.BetChips(blindPayment, handPot);
    }

    /*
     * Sabacc payment is also at beginning of round, every player pays this
     */
    private void PaySabaccPayment(Player player, int sabaccPayment)
    {
        // still need to add the animation
        player.BetChips(sabaccPayment, sabaccPot);

    }

    /*
     * player has picked a card to discard
     */
    public void PickedCard()
    {
        pickedCardToDiscard = true;
    }

    /*
     * The player has chosen to raise
     */
    private void RaiseChoice(Player player, int currentAmountToBet)
    {
        int callValue = currentAmountToBet - player.amountBettedThisRound;
        player.BetChips(callValue, handPot);

        UpdateTotalAmountBetted(player, callValue);
    }

    /*
     * Remove a game menu from sight
     */
    private void RemoveOptionsForRound(GameObject menu)
    {
        uiExit.GetComponent<AudioSource>().Play();
        menu.GetComponent<CanvasGroup>().DOFade(0, 1f);
        menu.GetComponent<CanvasGroup>().interactable = false;
        menu.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /*
     * Reveals the specific players hand
     */
    private void RevealPlayerHand(Player player)
    {
        player.revealCards = true;
        player.UpdateCurrentCardHand();
        cardFlipClip.GetComponent<AudioSource>().Play();
    }

    /*
     * Rolls the Sabacc Dice
     */
    private void RollDice()
    {
        sabaccDice.RollDice();
    }

    /*
     * Plays the animation for the SabaccPot Winning
     */
    private void SabaccPotAnimation(Player player, GamePot potToBetTo)
    {
        creditWinClip.GetComponent<AudioSource>().Play();
        creditImage.transform.position = new Vector3(potToBetTo.creditTotal.rectTransform.position.x, potToBetTo.creditTotal.rectTransform.position.y, potToBetTo.creditTotal.rectTransform.position.z);
        creditImage.SetActive(true);
        creditImage.transform.DOMove(new Vector2(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y), 0.35f);
    }

    /*
     * Set the player credit total
     */
    private void SetPlayersCreditTotal()
    {
        int index;
        for (index = 0; index < players.Count; index++)
        {
            players[index].credits = startingCredits;
        }
    }

    /*
     * Removes all cards from the players and then preforms a shuffle of the deck
     * I never remove cards from the deck itself I just make them inaccessable so 
     * this just reshuffles the deck that is already existing.
     */
    public IEnumerator ShuffleDeck()
    {
        int randomizedValue, index;

        int[] alreadyGeneratedValues = new int[deckSize];

        // wait to not immediately 'shuffle the deck'
        yield return new WaitForSeconds(2f);

        // if there is a card in the discard pile
        if (discardTopValue > 0)
        {
            Vector3 cachedDiscardLocation = new Vector3(topDiscardImage.transform.position.x, topDiscardImage.transform.position.y, topDiscardImage.transform.position.z);
            cardDiscardClip.GetComponent<AudioSource>().Play();
            topDiscardImage.transform.DOMove(new Vector2(deckVector3.transform.position.x, deckVector3.transform.position.y), 0.45f);
            topDiscardImage.enabled = false;

            yield return new WaitForSeconds(0.45f);

            topDiscardImage.enabled = true;
            topDiscardImage.sprite = currentDeck[deckSize - 1].ReturnCardBack();
            topDiscardImage.transform.position = cachedDiscardLocation;
        }

        for (index = 0; index < players.Count; index++)
        {
            if (players[index].cardHand.Count > 0)
            {
                Vector3 cachedCardLocation = new Vector3(cardToDiscard.transform.position.x, cardToDiscard.transform.position.y, cardToDiscard.transform.position.z);
                cardDiscardClip.GetComponent<AudioSource>().Play();

                // activate the card to discard and move it to the current players hand
                cardToDiscard.SetActive(true);
                cardToDiscard.transform.position = new Vector3(players[index].handLocation.transform.position.x, players[index].handLocation.transform.position.y, players[index].handLocation.transform.position.z);

                // moves card to the deck
                cardToDiscard.transform.DOMove(new Vector2(deckVector3.transform.position.x, deckVector3.transform.position.y), 0.45f);
                yield return new WaitForSeconds(0.45f);

                cardToDiscard.SetActive(false);
                cardToDiscard.transform.position = cachedCardLocation;

                // clear card hand
                players[index].cardHand.Clear();

                // Updates each players current card hand
                // pass in the player index so in Update Current Card Hand
                // I can set panel = players[index].handLocation
                // and current hand = to players[index].cardHand
                players[index].UpdateCurrentCardHand();
            }
        }

        Card[] tempDeck = currentDeck;
        discardPile = new Card[deckSize];
        currentDeck = new Card[deckSize];
        // I clear each hand in the above step

        // play shuffle sound
        shuffleCardsClip.GetComponent<AudioSource>().Play();

        // rotates back image of deck to give impression of shuffling
        backgroundDeckImage.transform.DORotate(new Vector3(backgroundDeckImage.transform.rotation.x, backgroundDeckImage.transform.rotation.y, backgroundDeckImage.transform.rotation.z + 360), 1f, RotateMode.FastBeyond360).SetRelative(true);

        for (index = 0; index < deckSize; index++)
        {
            alreadyGeneratedValues[index] = deckSize;
        }

        index = 0;
        while (index < deckSize)
        {
            randomizedValue = Random.Range(0, deckSize);
            if (!alreadyGeneratedValues.Contains<int>(randomizedValue))
            {
                alreadyGeneratedValues[index] = randomizedValue;
                currentDeck[index] = tempDeck[randomizedValue];
                index++;
            }
        }

        topDeckImage.sprite = currentDeck[deckSize - 1].ReturnCardBack();
        deckTopValue = deckSize - 1;
        topOfDeck = currentDeck[deckSize - 1];
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the player information based off of the Player Profile
        // Might do this in the Player Class "Grab the, done in the player profile
        playerAvaterImage.sprite = playerCharacter.GetPlayerImage();
        playerUsername.text = playerCharacter.GetPlayerName();

        // set deckSize, dosen't get changed after
        deckSize = preSabaccDeck.ReturnDeckSize();


        SetPlayersCreditTotal();

        // create the currentDeck for use throughout the game
        currentDeck = new Card[deckSize];

        // Initialize currentDeck with the cards from the sabacc deck
        InitializeDeck();

        // Shuffles the deck (or else it would be just cards in order, thats no fun!)
        StartCoroutine(ShuffleDeck());

        // starts the game that the player will play, only time this is called is at the start of this level
        StartCoroutine(StartGame());
    }

    /*
     * Main Game Loop, runs through the main phases of Sabacc,
     * these include betting, trading, and the dice round.
     */
    private IEnumerator StartGame()
    {
        // Variables for use throughout the function
        int playerWhoStarts = 0;
        int playerWhoCalledAlderaan = playerWhoStarts;
        int index, cards, playersWhoAreOut, playerWhoWonIndex = playerWhoStarts;
        int tempPlayer = playerWhoStarts;
        int winningPlayer;
        int sabaccPayment = 10;
        int blindPayment = 5;
        int currentAmountToBet = 0;
        int randomPlayer = Random.Range(0, players.Count);
        bool alderaan = false, everyoneChecked = false, noMoreBets;
        bool firstRound = true;
        bool sabaccVictory = false;
        bool newRound = false;

        // Tells the player the game has started
        gameInformation.text = "the game has started!";
        gameInformation.GetComponent<CanvasGroup>().alpha = 1;

        // initial wait to not immediately start the game
        yield return new WaitForSeconds(2.5f);

        gameInformation.text = "";

        // Plays a random voice phrase
        if (players[randomPlayer].hasVocalQueues)
        {
            players[randomPlayer].GameStart();
        }
        yield return new WaitForSeconds(1f);

        // The game has now STARTED!
        while (gameStates != sabaccGameStates.FINISHED)
        {
            // set players to be isOut if credits = 0, and reset 'folded'
            for(index = 0; index < players.Count; index++)
            {
                if(players[index].credits == 0)
                {
                    players[index].isOut = true;
                }
                players[index].folded = false;

                // reset the amount that each player has betted as its the beginning of a new round
                players[index].amountBettedThisRound = 0;

                // reset players hands
                int tempCardTotal = players[index].cardHand.Count;
                for(int tempIndex = 0; tempIndex < tempCardTotal; tempIndex++)
                {
                    DiscardCard(players[index], 0);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // blind pay and sabacc credit pot payment
            for(index = 0; index < players.Count; index++)
            {
                if (!players[index].isOut)
                {
                    if (index == MoveOntoNextPlayer(playerWhoStarts))
                    {
                        gameInformation.GetComponent<CanvasGroup>().alpha = 1;
                        gameInformation.text = players[index].thisPlayerName + " paid the blind!";

                        // play the betting animation
                        BetAnimation(players[index], handPot);

                        PayBlind(players[index], blindPayment);
                        // the person paying the blind is still betting
                        UpdateTotalAmountBetted(players[index], blindPayment);
                        currentAmountToBet = currentAmountToBet + blindPayment;

                        // after bet has completed wait seconds (0.35f)
                        // repeated code but this is the same as making a function
                        // as I need to repeat the WaitForSeconds here
                        yield return new WaitForSeconds(0.35f);
                        creditImage.SetActive(false);

                        yield return new WaitForSeconds(0.25f);
                    }

                    gameInformation.text = players[index].thisPlayerName + " paid the Sabacc payment.";

                    // play the betting animation
                    BetAnimation(players[index], sabaccPot);

                    PaySabaccPayment(players[index], sabaccPayment);

                    // after bet has completed wait seconds (0.35f)
                    // repeated code but this is the same as making a function
                    // as I need to repeat the WaitForSeconds here
                    yield return new WaitForSeconds(0.35f);
                    creditImage.SetActive(false);

                    yield return new WaitForSeconds(0.25f);
                }
            }

            // keeps going until newRound is true, (after winning
            newRound = false;
            firstRound = true;
            while (!newRound)
            {
                if (firstRound)
                {
                    gameInformation.text = "Dealing cards!";

                    tempPlayer = playerWhoStarts;

                    for (index = 0; index < players.Count; index++)
                    {
                        if (!players[tempPlayer].isOut)
                        {
                            // deal starting hand, needs to be not in a function so I can manipulate
                            // time
                            DealCard(players[tempPlayer]);
                            yield return new WaitForSeconds(0.30f);
                            topDeckImage.enabled = false;
                            yield return new WaitForSeconds(0.30f);
                            DealCardPart2(players[tempPlayer]);

                            DealCard(players[tempPlayer]);
                            yield return new WaitForSeconds(0.30f);
                            topDeckImage.enabled = false;
                            yield return new WaitForSeconds(0.30f);
                            DealCardPart2(players[tempPlayer]);
                        }
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }
                }

                tempPlayer = playerWhoStarts;
                noMoreBets = false;

                gameInformation.text = "Now we are on the Betting Round!";
                yield return new WaitForSeconds(0.35f);

                // betting round
                while (!noMoreBets)
                {
                    everyoneChecked = true;
                    for (index = 0; index < players.Count; index++)
                    {
                        if (!players[tempPlayer].isOut && !players[tempPlayer].folded && !players[tempPlayer].isAllIn)
                        {
                            gameInformation.text = players[index].thisPlayerName + " is choosing what to do!";

                            if (players[tempPlayer].playerType == Player.PlayerType.PLAYER)
                            {
                                callValueText.text = "" + (currentAmountToBet - players[tempPlayer].amountBettedThisRound);
                                raiseValueText.text = "" + ((currentAmountToBet * 2) - players[tempPlayer].amountBettedThisRound);
                                allInValueText.text = "" + players[tempPlayer].credits;

                                DisplayOptionsForRound(bettingMenu);
                                yield return new WaitUntil(() => buttonPressed == true);
                                RemoveOptionsForRound(bettingMenu);
                            }
                            else if (players[tempPlayer].playerType == Player.PlayerType.AI)
                            {
                                GetBestAIBetOption(players[tempPlayer], currentAmountToBet);
                            }

                            yield return new WaitForSeconds(0.35f);


                            // Safety catch for player being dumb, (me, did this a couple times)
                            if (players[tempPlayer].betChoice == Player.BettingChoice.CHECK && (currentAmountToBet - players[tempPlayer].amountBettedThisRound) > 0)
                            {
                                players[tempPlayer].betChoice = Player.BettingChoice.CALL;

                                gameInformation.text = players[tempPlayer].thisPlayerName + " chose CHECK when they have to at least CALL!";

                                yield return new WaitForSeconds(2f);
                            }

                            if(players[tempPlayer].credits < currentAmountToBet && players[tempPlayer].betChoice == Player.BettingChoice.CALL)
                            {
                                players[tempPlayer].betChoice = Player.BettingChoice.ALLIN;
                            }
                            if (players[tempPlayer].credits < currentAmountToBet*2 && players[tempPlayer].betChoice == Player.BettingChoice.RAISE)
                            {
                                players[tempPlayer].betChoice = Player.BettingChoice.ALLIN;
                            }

                            /*
                             * BETTING SECTION
                             * This section checks to see what choice either the PLAYER or AI
                             * players have chosen, both should work the same way so I set them
                             * up for the same way
                             */

                            // check for what the betChoice is for the current player and do the thing
                            // based off of choice
                            if (players[tempPlayer].betChoice == Player.BettingChoice.CALL)
                            {
                                // play the betting animation
                                BetAnimation(players[tempPlayer], handPot);

                                
                                gameInformation.text = players[tempPlayer].thisPlayerName + " called!";
                                everyoneChecked = false;
                                CallChoice(players[tempPlayer], currentAmountToBet);

                                yield return new WaitForSeconds(0.30f);
                                creditImage.SetActive(false);
                            }
                            else if (players[tempPlayer].betChoice == Player.BettingChoice.CHECK)
                            {
                                
                                gameInformation.text = players[tempPlayer].thisPlayerName + " checked!";
                            }
                            else if (players[tempPlayer].betChoice == Player.BettingChoice.RAISE)
                            {
                                // play the betting animation
                                BetAnimation(players[tempPlayer], handPot);

                                
                                gameInformation.text = players[tempPlayer].thisPlayerName + " raised!";
                                everyoneChecked = false;
                                currentAmountToBet = currentAmountToBet * 2;
                                RaiseChoice(players[tempPlayer], currentAmountToBet);

                                yield return new WaitForSeconds(0.30f);
                                creditImage.SetActive(false);
                            }
                            else if (players[tempPlayer].betChoice == Player.BettingChoice.ALLIN)
                            {
                                if (players[tempPlayer].hasVocalQueues)
                                {
                                    players[tempPlayer].PlayGood();
                                }
                                yield return new WaitForSeconds(0.55f);

                                // play the betting animation
                                BetAnimation(players[tempPlayer], handPot);

                                
                                gameInformation.text = players[tempPlayer].thisPlayerName + " is going all in!";
                                everyoneChecked = false;

                                AllInChoice(players[tempPlayer]);

                                yield return new WaitForSeconds(0.35f);
                                creditImage.SetActive(false);
                            }
                            else if (players[tempPlayer].betChoice == Player.BettingChoice.FOLD)
                            {
                                // display discard animation
                                if (players[tempPlayer].hasVocalQueues)
                                {
                                    players[tempPlayer].PlayBad();
                                }
                                yield return new WaitForSeconds(0.55f);

                                Debug.Log("Fold choice was chosen");
                                gameInformation.text = players[tempPlayer].thisPlayerName + " has folded!";
                                everyoneChecked = false;

                                int tempIndex, cachedCardCount = players[tempPlayer].cardHand.Count;

                                players[tempPlayer].folded = true;

                                for (tempIndex = 0; tempIndex < cachedCardCount; tempIndex++)
                                {
                                    DiscardCard(players[tempPlayer], 0);
                                    yield return new WaitForSeconds(0.5f);
                                }
                            }

                            buttonPressed = false;
                            yield return new WaitForSeconds(1.5f);
                            tempPlayer = MoveOntoNextPlayer(tempPlayer);
                        }
                        else
                        {
                            tempPlayer = MoveOntoNextPlayer(tempPlayer);
                        }
                    }
                    if (everyoneChecked == true)
                    {
                        noMoreBets = true;
                    }
                }

                gameInformation.text = "Now we are on the Trading Round!";
                yield return new WaitForSeconds(0.35f);

                // trading round
                tempPlayer = playerWhoStarts;
                for (index = 0; index < players.Count; index++)
                {
                    if (!players[tempPlayer].isOut && !players[tempPlayer].folded)
                    {
                        gameInformation.text = players[tempPlayer].thisPlayerName + "is choosing what to do!";

                        if (players[tempPlayer].playerType == Player.PlayerType.PLAYER)
                        {
                            pickedCardToDiscard = false;
                            discardOn = false;
                            DisplayOptionsForRound(tradingRound);
                            yield return new WaitUntil(() => buttonPressed == true);

                            RemoveOptionsForRound(tradingRound);
                        }
                        else if (players[tempPlayer].playerType == Player.PlayerType.AI)
                        {
                            GetBestAITradeOption(players[tempPlayer]);
                        }

                        yield return new WaitForSeconds(0.35f);

                        if (players[tempPlayer].tradeChoice == Player.TradingChoice.ALDERAAN && firstRound == true)
                        {
                            players[tempPlayer].tradeChoice = Player.TradingChoice.STAND;
                            gameInformation.text = players[index].thisPlayerName + " chose ALDERAAN on the FIRST TURN, they will STAND instead!";
                            yield return new WaitForSeconds(2f);
                        }

                        // check for what the betChoice is for the current player and do the thing
                        // based off of choice
                        if (players[tempPlayer].tradeChoice == Player.TradingChoice.ADD)
                        {
                            gameInformation.text = players[tempPlayer].thisPlayerName + " has chosen to add a card!";
                            yield return new WaitForSeconds(0.5f);

                            

                            DealCard(players[tempPlayer]);
                            yield return new WaitForSeconds(0.35f);
                            topDeckImage.enabled = false;
                            yield return new WaitForSeconds(0.35f);
                            DealCardPart2(players[tempPlayer]);
                        }
                        else if (players[tempPlayer].tradeChoice == Player.TradingChoice.TRADE)
                        {
                            
                            gameInformation.text = players[tempPlayer].thisPlayerName + " has chosen to trade a card!";
                            yield return new WaitForSeconds(0.45f);

                            discardOn = true;
                            if (players[tempPlayer].playerType == Player.PlayerType.PLAYER)
                            {
                                gameInformation.text = "Click a card to trade!";
                                yield return new WaitUntil(() => pickedCardToDiscard == true);

                                DealCard(players[tempPlayer]);
                                yield return new WaitForSeconds(0.35f);
                                topDeckImage.enabled = false;
                                yield return new WaitForSeconds(0.35f);
                                DealCardPart2(players[tempPlayer]);
                            }
                            else
                            {
                                int tempCardToDiscard = 0;
                                int highestCardValue = players[tempPlayer].cardHand[0].ReturnCardValue();

                                for (int tempIndex = 0; tempIndex < players[tempPlayer].cardHand.Count; tempIndex++)
                                {
                                    if (players[tempPlayer].cardHand[tempIndex].ReturnCardValue() >= highestCardValue)
                                    {
                                        tempCardToDiscard = tempIndex;
                                        highestCardValue = players[tempPlayer].cardHand[tempIndex].ReturnCardValue();
                                    }
                                }

                                DiscardCard(players[tempPlayer], tempCardToDiscard);

                                DealCard(players[tempPlayer]);
                                yield return new WaitForSeconds(0.35f);
                                topDeckImage.enabled = false;
                                yield return new WaitForSeconds(0.35f);
                                DealCardPart2(players[tempPlayer]);
                            }
                        }
                        else if (players[tempPlayer].tradeChoice == Player.TradingChoice.STAND)
                        {
                            gameInformation.text = players[tempPlayer].thisPlayerName + " has chosen to stand!";
                            yield return new WaitForSeconds(0.45f);

                            
                        }
                        else if (players[tempPlayer].tradeChoice == Player.TradingChoice.ALDERAAN)
                        {
                            if (players[tempPlayer].hasVocalQueues)
                            {
                                players[tempPlayer].PlayVictory();
                            }
                            yield return new WaitForSeconds(1f);

                            gameInformation.text = players[tempPlayer].thisPlayerName + " has called ALDERAAN!";
                            yield return new WaitForSeconds(0.5f);

                            alderaan = true;
                        }

                        buttonPressed = false;
                        yield return new WaitForSeconds(2f);
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }

                    else
                    {
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }

                }

                // check for alderaan
                tempPlayer = playerWhoStarts;
                if (alderaan)
                {
                    alderaan = false;

                    gameInformation.text = "Time for Alderaan!";
                    yield return new WaitForSeconds(0.75f);

                    // reveal all AI players hands
                    for (index = 0; index < players.Count; index++)
                    {
                        if (!players[tempPlayer].isOut && !players[tempPlayer].folded)
                        {
                            if (players[tempPlayer].playerType == Player.PlayerType.AI)
                            {
                                RevealPlayerHand(players[tempPlayer]);
                                yield return new WaitForSeconds(0.25f);
                            }
                        }
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }

                    gameInformation.text = "Hands revealed!";
                    yield return new WaitForSeconds(7.5f);

                    for (index = 0; index < players.Count; index++)
                    {
                        HidePlayerHand(players[index]);
                    }

                    // reset starting player
                    tempPlayer = playerWhoStarts;

                    gameInformation.text = "Checking who won!";
                    yield return new WaitForSeconds(2f);

                    // check to see the winning player
                    winningPlayer = tempPlayer;
                    for (index = 0; index < players.Count; index++)
                    {
                        if (!players[tempPlayer].isOut && !players[tempPlayer].folded)
                        {
                            gameInformation.text = players[tempPlayer].thisPlayerName + "total score = " + players[tempPlayer].handValue;
                            yield return new WaitForSeconds(4f);

                            // bomb out cause of zero
                            if (players[tempPlayer].handValue == 0)
                            {
                                if (players[tempPlayer].hasVocalQueues)
                                {
                                    players[tempPlayer].PlayDefeat();
                                }
                                yield return new WaitForSeconds(1f);

                                BetAnimation(players[tempPlayer], sabaccPot);
                                BombOut(players[tempPlayer]);

                                yield return new WaitForSeconds(0.35f);
                                creditImage.SetActive(false);

                                gameInformation.text = players[tempPlayer].thisPlayerName + " has bombed out with score of 0!";
                                yield return new WaitForSeconds(1.5f);
                            }

                            // bomb out cause bigger than 23 or less than -23
                            else if (players[tempPlayer].handValue > 23 || players[tempPlayer].handValue < -23)
                            {
                                if (players[tempPlayer].hasVocalQueues)
                                {
                                    players[tempPlayer].PlayDefeat();
                                }
                                yield return new WaitForSeconds(1f);

                                BetAnimation(players[tempPlayer], sabaccPot);

                                BombOut(players[tempPlayer]);

                                yield return new WaitForSeconds(0.35f);
                                creditImage.SetActive(false);

                                gameInformation.text = players[tempPlayer].thisPlayerName + " has bombed out with a score greater than -23 or 23!";
                                yield return new WaitForSeconds(1.5f);
                            }
                            else if (players[tempPlayer].idiotsArray == true)
                            {
                                sabaccVictory = true;
                                winningPlayer = tempPlayer;
                            }
                            else if (players[tempPlayer].fairyEmpress == true)
                            {
                                sabaccVictory = true;
                                winningPlayer = tempPlayer;
                            }
                            else if (players[tempPlayer].sabacc == true)
                            {
                                sabaccVictory = true;
                                winningPlayer = tempPlayer;
                            }
                            else if (players[tempPlayer].distanceFrom23 <= players[winningPlayer].distanceFrom23)
                            {
                                if (!sabaccVictory)
                                {
                                    sabaccVictory = false;
                                    winningPlayer = tempPlayer;
                                }
                            }
                            
                        }
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }
                    if (sabaccVictory == true)
                    {
                        if (players[winningPlayer].hasVocalQueues)
                        {
                            players[winningPlayer].PlayVictory();
                        }
                        yield return new WaitForSeconds(1f);

                        SabaccPotAnimation(players[winningPlayer], sabaccPot);
                        yield return new WaitForSeconds(0.35f);
                        creditImage.SetActive(false);

                        players[winningPlayer].credits = players[winningPlayer].credits + sabaccPot.WonPot();
                        sabaccPot.creditTotal.text = "" + 0;

                        HandPotAnimation(players[winningPlayer], handPot);
                        yield return new WaitForSeconds(0.35f);
                        creditImage.SetActive(false);
                        players[winningPlayer].credits = players[winningPlayer].credits + handPot.WonPot();
                        handPot.creditTotal.text = "" + 0;

                        gameInformation.text = players[winningPlayer].thisPlayerName + " wins with " + players[winningPlayer].handValue + " which is a sabacc!";
                        yield return new WaitForSeconds(6f);
                    }
                    else
                    {
                        if (players[winningPlayer].hasVocalQueues)
                        {
                            players[winningPlayer].PlayVictory();
                        }
                        yield return new WaitForSeconds(1f);

                        players[winningPlayer].credits = players[winningPlayer].credits + handPot.WonPot();
                        handPot.creditTotal.text = "" + 0;
                        HandPotAnimation(players[winningPlayer], handPot);
                        yield return new WaitForSeconds(0.35f);
                        creditImage.SetActive(false);

                        gameInformation.text = players[winningPlayer].thisPlayerName + " wins with " + players[winningPlayer].handValue;
                        yield return new WaitForSeconds(6f);
                    }

                    for (index = 0; index < players.Count; index++)
                    {
                        if (players[index].credits == 0)
                        {
                            players[index].isOut = true;
                            Debug.Log(players[index].thisPlayerName + " is now out");
                        }
                        
                    }

                    // check to see if a player is last standing
                    // end game if so
                    sabaccVictory = false;
                    playerWhoStarts = MoveOntoNextPlayer(playerWhoStarts);
                    playersWhoAreOut = 0;

                    for (index = 0; index < players.Count; index++)
                    {
                        if (players[index].isOut)
                        {
                            playersWhoAreOut++;
                        }
                        else
                        {
                            playerWhoWonIndex = index;
                        }
                    }

                    Debug.Log(playersWhoAreOut + "SHOULD WORK!");
                    if (playersWhoAreOut == players.Count - 1)
                    {
                        gameStates = MainGameplaySabacc.sabaccGameStates.FINISHED;

                        gameInformation.text = "Game Over!";
                        yield return new WaitForSeconds(3f);
                    }

                    // Start the new round
                    firstRound = true;
                    newRound = true;
                }
                // Sabacc shift, no alderaan
                else
                {
                    gameInformation.text = "Time for the Sabaac Shift!";
                    yield return new WaitForSeconds(0.5f);

                    DisplayOptionsForRound(diceRound);
                    yield return new WaitForSeconds(2f);
                    RollDice();
                    yield return new WaitForSeconds(4f);

                    tempPlayer = playerWhoStarts;

                    if (sabaccDice.sabaccShift)
                    {
                        for (index = 0; index < players.Count; index++)
                        {
                            if (!players[tempPlayer].isOut && !players[tempPlayer].folded)
                            {
                                // gets the amount of cards for each player
                                players[tempPlayer].cardsForSabaccShift = players[tempPlayer].cardHand.Count;
                            }
                            tempPlayer = MoveOntoNextPlayer(tempPlayer);
                        }

                        tempPlayer = playerWhoStarts;
                        for (index = 0; index < players.Count; index++)
                        {
                            if (!players[tempPlayer].isOut && !players[tempPlayer].folded)
                            {
                                for (cards = 0; cards < players[tempPlayer].cardsForSabaccShift; cards++)
                                {
                                    DiscardCard(players[index], 0);
                                    yield return new WaitForSeconds(0.5f);
                                }

                                for (cards = 0; cards < players[tempPlayer].cardsForSabaccShift; cards++)
                                {
                                    DealCard(players[tempPlayer]);
                                    yield return new WaitForSeconds(0.35f);
                                    topDeckImage.enabled = false;
                                    yield return new WaitForSeconds(0.35f);
                                    DealCardPart2(players[tempPlayer]);
                                }
                            }
                        }
                    }
                    sabaccDice.sabaccShift = false;

                    yield return new WaitForSeconds(0.5f);
                    RemoveOptionsForRound(diceRound);
                    yield return new WaitForSeconds(0.5f);
                    firstRound = false;
                }
                
            }
            // exits out of loop where Game Winning Screen is displayed
        }
        DisplayGameWinningScreen(players[playerWhoWonIndex]);
        // from here player can choose to exit or restart the level
    }

    private void UpdateTotalAmountBetted(Player player, int totalBetted)
    {
        player.amountBettedThisRound = player.amountBettedThisRound + totalBetted;
    }
}
