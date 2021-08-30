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

    [Title("Game Pots")]
    [SerializeField] GamePot sabaccPot;
    [SerializeField] GamePot handPot;

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

    [Title("Starting Credits")]
    [SerializeField] int startingCredits = 2000;

    [Title("Credit Animation Items")]
    [SerializeField] GameObject creditImage;
    [SerializeField] GameObject creditExchangeClip;
    [SerializeField] GameObject creditWinClip;

    [Title("Game Informer")]
    [SerializeField] TextMeshProUGUI gameInformation;

    [Title("Betting Round Text for Player")]
    [SerializeField] TextMeshProUGUI callValueText;
    [SerializeField] TextMeshProUGUI raiseValueText;
    [SerializeField] TextMeshProUGUI allInValueText;

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

        InitializeDeck();

        StartCoroutine(ShuffleDeck());

        StartCoroutine(StartGame());
    }

    private void SetPlayersCreditTotal()
    {
        int index;
        for(index = 0; index < players.Count; index++)
        {
            players[index].credits = startingCredits;
        }
    }

    public IEnumerator StartGame()
    {
        int playerWhoStarts = 0;
        int playerWhoCalledAlderaan = playerWhoStarts;
        int index, cards, playersWhoAreOut, playerWhoWonIndex = playerWhoStarts;
        int tempPlayer = playerWhoStarts;
        int winningPlayer;
        int sabaccPayment = 10;
        int blindPayment = 5;
        int currentAmountToBet = 0;
        bool alderaan = false, everyoneChecked = false, noMoreBets;
        bool firstRound = true;
        bool sabaccVictory = false;
        bool newRound = false;

        gameInformation.text = "the game has started!";
        gameInformation.GetComponent<CanvasGroup>().alpha = 1;

        // initial wait to not immidiately start the game
        yield return new WaitForSeconds(2.5f);

        gameInformation.text = "";

        int randomPlayer = Random.Range(0, players.Count);

        if (players[randomPlayer].hasVocalQueues)
        {
            players[randomPlayer].GameStart();
        }
        yield return new WaitForSeconds(1f);

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

                        yield return new WaitForSeconds(0.65f);
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

                    yield return new WaitForSeconds(0.65f);
                }
            }

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
                            yield return new WaitForSeconds(0.35f);
                            topDeckImage.enabled = false;
                            yield return new WaitForSeconds(0.35f);
                            DealCardPart2(players[tempPlayer]);

                            DealCard(players[tempPlayer]);
                            yield return new WaitForSeconds(0.35f);
                            topDeckImage.enabled = false;
                            yield return new WaitForSeconds(0.35f);
                            DealCardPart2(players[tempPlayer]);
                        }
                        tempPlayer = MoveOntoNextPlayer(tempPlayer);
                    }
                }

                tempPlayer = playerWhoStarts;
                noMoreBets = false;

                gameInformation.text = "Now we are on the Betting Round!";
                yield return new WaitForSeconds(0.5f);

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

                            yield return new WaitForSeconds(0.5f);


                            // Safety catch for player being dumb, (me, did this a couple times)
                            if (players[tempPlayer].betChoice == Player.BettingChoice.CHECK && (currentAmountToBet - players[tempPlayer].amountBettedThisRound) > 0)
                            {
                                players[tempPlayer].betChoice = Player.BettingChoice.CALL;

                                gameInformation.text = players[tempPlayer].thisPlayerName + " chose CHECK when they have to at least CALL!";

                                yield return new WaitForSeconds(3f);
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

                                yield return new WaitForSeconds(0.35f);
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

                                yield return new WaitForSeconds(0.35f);
                                creditImage.SetActive(false);
                            }
                            else if (players[tempPlayer].betChoice == Player.BettingChoice.ALLIN)
                            {
                                if (players[tempPlayer].hasVocalQueues)
                                {
                                    players[tempPlayer].PlayGood();
                                }
                                yield return new WaitForSeconds(1f);

                                // play the betting animation
                                BetAnimation(players[tempPlayer], handPot);

                                
                                gameInformation.text = players[tempPlayer].thisPlayerName + " is going all in!";
                                everyoneChecked = false;

                                currentAmountToBet = currentAmountToBet + players[tempPlayer].credits;
                                AllInChoice(players[tempPlayer], currentAmountToBet);

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
                                yield return new WaitForSeconds(1f);

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
                            yield return new WaitForSeconds(2f);
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
                yield return new WaitForSeconds(0.5f);

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

                        yield return new WaitForSeconds(0.5f);

                        if (players[tempPlayer].tradeChoice == Player.TradingChoice.ALDERAAN && firstRound == true)
                        {
                            players[tempPlayer].tradeChoice = Player.TradingChoice.STAND;
                            gameInformation.text = players[index].thisPlayerName + " chose ALDERAAN on the FIRST TURN, they will STAND instead!";
                            yield return new WaitForSeconds(3f);
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
                            yield return new WaitForSeconds(0.5f);

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
                                int highestCardValue = players[tempPlayer].cardHand[0].cardValue;

                                for (int tempIndex = 0; tempIndex < players[tempPlayer].cardHand.Count; tempIndex++)
                                {
                                    if (players[tempPlayer].cardHand[tempIndex].cardValue >= highestCardValue)
                                    {
                                        tempCardToDiscard = tempIndex;
                                        highestCardValue = players[tempPlayer].cardHand[tempIndex].cardValue;
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
                            yield return new WaitForSeconds(0.5f);

                            
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
                        sabaccPot.credit_total.text = "" + 0;

                        HandPotAnimation(players[winningPlayer], handPot);
                        yield return new WaitForSeconds(0.35f);
                        creditImage.SetActive(false);
                        players[winningPlayer].credits = players[winningPlayer].credits + handPot.WonPot();
                        handPot.credit_total.text = "" + 0;

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
                        handPot.credit_total.text = "" + 0;
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

    private void HandPotAnimation(Player player, GamePot potToBetTo)
    {
        creditWinClip.GetComponent<AudioSource>().Play();
        creditImage.transform.position = new Vector3(potToBetTo.credit_total.rectTransform.position.x, potToBetTo.credit_total.rectTransform.position.y, potToBetTo.credit_total.rectTransform.position.z);
        creditImage.SetActive(true);
        creditImage.transform.DOMove(new Vector2(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y), 0.35f);
    }

    private void SabaccPotAnimation(Player player, GamePot potToBetTo)
    {
        creditWinClip.GetComponent<AudioSource>().Play();
        creditImage.transform.position = new Vector3(potToBetTo.credit_total.rectTransform.position.x, potToBetTo.credit_total.rectTransform.position.y, potToBetTo.credit_total.rectTransform.position.z);
        creditImage.SetActive(true);
        creditImage.transform.DOMove(new Vector2(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y), 0.35f);
    }

    public void PickedCard()
    {
        pickedCardToDiscard = true;
    }

    private void BetAnimation(Player player, GamePot potToBetTo)
    {
        creditExchangeClip.GetComponent<AudioSource>().Play();
        creditImage.transform.position = new Vector3(player.GetCreditText().rectTransform.position.x, player.GetCreditText().rectTransform.position.y, player.GetCreditText().rectTransform.position.z);
        creditImage.SetActive(true);
        creditImage.transform.DOMove(new Vector2(potToBetTo.credit_total.rectTransform.position.x, potToBetTo.credit_total.rectTransform.position.y), 0.35f);
    }

    private void UpdateTotalAmountBetted(Player player, int totalBetted)
    {
        player.amountBettedThisRound = player.amountBettedThisRound + totalBetted;
    }

    private void DisplayGameWinningScreen(Player player)
    {
        if(player.hasVocalQueues)
        {
            player.PlayVictory();
        }
        playerWinsText.text = player.thisPlayerName + " wins!";
        gameOverScreen.GetComponent<CanvasGroup>().DOFade(1, 1f);
        gameOverScreen.GetComponent<CanvasGroup>().interactable = true;
        gameOverScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ButtonPressed()
    {
        buttonPressed = true;
    }

    private void DealCard(Player player)
    {
        player.cardHand.Add(topOfDeck);


        topDeckImage.rectTransform.DOMove(new Vector2(player.handLocation.transform.position.x, player.handLocation.transform.position.y), 0.35f);

        addCardClip.GetComponent<AudioSource>().Play();
    }

    private void DealCardPart2(Player player)
    {
        // from DealCardAnimation
        topDeckImage.rectTransform.position = deckVector3.transform.position;
        topDeckImage.enabled = true;

        deckTopValue = deckTopValue - 1;
        topOfDeck = currentDeck[deckTopValue];
        // set this to cardFace to see image of what card to be next
        topDeckImage.sprite = topOfDeck.cardBack;

        player.UpdateCurrentCardHand();
    }
    
    private void RollDice()
    {
        sabaccDice.RollDice();
    }

    private void BombOut(Player player)
    {
        int bombOutAmount = handPot.pot_total / 10;

        if(bombOutAmount > player.credits)
        {
            bombOutAmount = player.credits;
        }

        player.BetChips(bombOutAmount, sabaccPot);
    }

    private void RevealPlayerHand(Player player)
    {
        player.revealCards = true;
        player.UpdateCurrentCardHand();
        cardFlipClip.GetComponent<AudioSource>().Play();
    }

    private void HidePlayerHand(Player player)
    {
        player.revealCards = false;
        player.UpdateCurrentCardHand();
        cardFlipClip.GetComponent<AudioSource>().Play();
    }

    private void GetBestAITradeOption(Player player)
    {
        int random = Random.Range(1, 10);

        if (player.distanceFrom23 > 15)
        {
            player.tradeChoice = Player.TradingChoice.TRADE;
        }
        else if(player.distanceFrom23 > 7)
        {
            player.tradeChoice = Player.TradingChoice.ADD;
        }
        else if(player.distanceFrom23 > 2)
        {
            player.tradeChoice = Player.TradingChoice.STAND;
        }
        else
        {
            if(random < 8)
            {
                player.tradeChoice = Player.TradingChoice.STAND;
            }
            else
            {
                player.tradeChoice = Player.TradingChoice.ALDERAAN;
            }
        }
    }

    private void AllInChoice(Player player, int currentAmountToBet)
    {
        //int callValue = currentAmountToBet - player.amountBettedThisRound;
        player.BetChips(player.credits, handPot);

        // MAKE SURE TO CHANGE THIS *******
        UpdateTotalAmountBetted(player, player.credits);
    }

    private void RaiseChoice(Player player, int currentAmountToBet)
    {
        int callValue = currentAmountToBet - player.amountBettedThisRound;
        player.BetChips(callValue, handPot);

        // MAKE SURE TO CHANGE THIS *******
        UpdateTotalAmountBetted(player, callValue);
    }

    private void CallChoice(Player player, int currentAmountToBet)
    {
        int callValue = currentAmountToBet - player.amountBettedThisRound;
        player.BetChips(callValue, handPot);

        // MAKE SURE TO CHANGE THIS *******
        UpdateTotalAmountBetted(player, callValue);
    }

    private void GetBestAIBetOption(Player player, int currentAmountToBet)
    {
        int random = Random.Range(0, player.distanceFrom23 + 4);
        int callValue = currentAmountToBet - player.amountBettedThisRound;

        if(player.distanceFrom23 >= 20)
        {
            player.betChoice = Player.BettingChoice.FOLD;
        }
        else if(random == player.distanceFrom23 && player.distanceFrom23 >= 1 && player.distanceFrom23 <=5)
        {
            player.betChoice = Player.BettingChoice.RAISE;
        }
        else if(player.distanceFrom23 == 0 && random == 0)
        {
            player.betChoice = Player.BettingChoice.ALLIN;
        }
        else
        {
            if (callValue == 0)
            {
                player.betChoice = Player.BettingChoice.CHECK;
            }
            else
            {
                player.betChoice = Player.BettingChoice.CALL;
            }
        }
    }

    private void DisplayOptionsForRound(GameObject menu)
    {
        uiPrompt.GetComponent<AudioSource>().Play();
        menu.GetComponent<CanvasGroup>().DOFade(1, 1f);
        menu.GetComponent<CanvasGroup>().interactable = true;
        menu.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    
    private void RemoveOptionsForRound(GameObject menu)
    {
        uiExit.GetComponent<AudioSource>().Play();
        menu.GetComponent<CanvasGroup>().DOFade(0, 1f);
        menu.GetComponent<CanvasGroup>().interactable = false;
        menu.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void DiscardCard(Player player, int cardIndexToDiscard)
    {
        // grab cardFace image so we can discard the card from the hand
        Sprite cachedSprite = player.cardHand[cardIndexToDiscard].cardFace;

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

    IEnumerator DiscardCardAnimation(float seconds, int cardIndexToDiscard, Vector3 cachedDiscardLocation)
    {
        yield return new WaitForSeconds(seconds);

        cardToDiscard.SetActive(false);
        cardToDiscard.transform.position = cachedDiscardLocation;
    }

    private void PaySabaccPayment(Player player, int sabaccPayment)
    {
        // still need to add the animation
        player.BetChips(sabaccPayment, sabaccPot);
        
    }

    private void PayBlind(Player player, int blindPayment)
    {
        // move the credit chip
        // still need to add the animation
        // reduce player chip count and add it to the pot
        player.BetChips(blindPayment, handPot);
    }

    private int MoveOntoNextPlayer(int player)
    {
        int nextPlayer = player + 1;
        if (nextPlayer >= players.Count)
        {
            nextPlayer = 0;
        }
        return nextPlayer;
    }

    public void InitializeDeck()
    {
        int index;
        for( index = 0; index < deckSize; index++ )
        {
            // assigns every currentDeck index = to the preSabaccDeck
            currentDeck[index] = preSabaccDeck.GetIndex(index);
        }

        // set to cardFace to see if what the top card is
        topDeckImage.sprite = currentDeck[deckSize - 1].cardBack;
        deckTopValue = deckSize - 1;
        topOfDeck = currentDeck[deckSize - 1];
    }

    public IEnumerator ShuffleDeck()
    {
        int randomizedValue, index;

        int[] alreadyGeneratedValues = new int[deckSize];

        // wait to not immidiately 'shuffle the deck'
        yield return new WaitForSeconds(2f);

        // if there is a card in the discard pile
        if(discardTopValue > 0)
        {
            Vector3 cachedDiscardLocation = new Vector3(topDiscardImage.transform.position.x, topDiscardImage.transform.position.y, topDiscardImage.transform.position.z);
            cardDiscardClip.GetComponent<AudioSource>().Play();
            topDiscardImage.transform.DOMove(new Vector2(deckVector3.transform.position.x, deckVector3.transform.position.y), 0.45f);
            topDiscardImage.enabled = false;

            yield return new WaitForSeconds(0.45f);

            topDiscardImage.enabled = true;
            topDiscardImage.sprite = currentDeck[deckSize - 1].cardBack;
            topDiscardImage.transform.position = cachedDiscardLocation;
        }

        for (index = 0; index< players.Count; index++)
        {
            if(players[index].cardHand.Count > 0)
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
                //GetComponent<PlayerHand>().UpdateCurrentCardHand(players[index]);
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

        for(index = 0; index < deckSize; index++)
        {
            alreadyGeneratedValues[index] = deckSize;
        }

        index = 0;
        while(index < deckSize)
        {
            randomizedValue = Random.Range(0, deckSize);
            if(!alreadyGeneratedValues.Contains<int>(randomizedValue))
            {
                alreadyGeneratedValues[index] = randomizedValue;
                currentDeck[index] = tempDeck[randomizedValue];
                index++;
            }
        }

        topDeckImage.sprite = currentDeck[deckSize - 1].cardBack;
        deckTopValue = deckSize - 1;
        topOfDeck = currentDeck[deckSize - 1];
    }
}
