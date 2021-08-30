using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
    public string thisPlayerName;
    [SerializeField] TextMeshProUGUI playersTextLocation;

    public Sprite thisPlayerAvatar;
    [SerializeField] Image playersImageLocation;

    [SerializeField] public GameObject handLocation;

    [Title("For Card Hand of Current Player")]
    public List<Card> cardHand;
    [SerializeField] TextMeshProUGUI totalValue;
    GameObject cardToAdd;
    Image cardsImage;

    public bool isOut = false;
    public bool isAllIn = false;

    public bool folded = false;

    public int credits = 20000;
    [SerializeField] TextMeshProUGUI creditText;

    public int handValue;

    public int distanceFrom23;

    public int cardsForSabaccShift;

    public bool sabacc = false;
    public bool idiotsArray = false;
    public bool fairyEmpress = false;

    public bool revealCards = false;

    public int amountBettedThisRound;

    [SerializeField] Material playerIsOutMaterial;

    [Title("Vocal Queues")]
    [SerializeField] public bool hasVocalQueues;
    [SerializeField] List<AudioClip> victory;
    [SerializeField] List<AudioClip> defeat;
    [SerializeField] List<AudioClip> good;
    [SerializeField] List<AudioClip> bad;
    [SerializeField] List<AudioClip> gameStart;
    [SerializeField] AudioSource playerVocalQueues;

    public BettingChoice betChoice;
    public PlayerType playerType;
    public TradingChoice tradeChoice;

    public enum PlayerType
    {
        AI,
        PLAYER
    };

    public enum BettingChoice
    {
        CALL,
        CHECK,
        RAISE,
        ALLIN,
        FOLD
    };

    public enum TradingChoice
    {
        ADD,
        TRADE,
        STAND,
        ALDERAAN
    };

    private void Start()
    { 
        playersTextLocation.text = thisPlayerName;
        playersImageLocation.sprite = thisPlayerAvatar;
        isAllIn = false;
    }

    private void Update()
    {
        if(isOut)
        {
            playersImageLocation.material = playerIsOutMaterial;
        }
        GetHandValue();
        //UpdateCurrentCardHand();
        creditText.text = "" + credits;

        if(credits <= 0)
        {
            credits = 0;
            isAllIn = true;
        }
    }

    public TextMeshProUGUI GetCreditText()
    {
        return creditText;
    }

    public void GetHandValue()
    {
        int index, tempHandValue = 0;

        for(index = 0; index < cardHand.Count; index++)
        {
            tempHandValue = tempHandValue + cardHand[index].cardValue;
        }

        // set the hand value
        handValue = tempHandValue;
        
        // set distance from 23

        // negative
        if(handValue < 0)
        {
            distanceFrom23 = 23 - (handValue * -1);
        }
        // positive
        else
        {
            distanceFrom23 = 23 - handValue;
        }

        // check for sabacc
        if(distanceFrom23 == 0)
        {
            sabacc = true;
        }
        else
        {
            sabacc = false;
        }

        // check for specialSuits

        // fairy empress
        if(cardHand.Count == 2)
        {
            if(cardHand[0].cardName == "The Queen of Air and Darkness" && cardHand[1].cardName == "The Queen of Air and Darkness")
            {
                fairyEmpress = true;
            }
            else
            {
                fairyEmpress = false;
            }
        }

        // idiots array
        if(cardHand.Count == 3)
        {
            if(cardHand[0].cardName == "The Idiot" || cardHand[1].cardName == "The Idiot" || cardHand[2].cardName == "The Idiot")
            {
                if(cardHand[0].cardName == "Two" || cardHand[1].cardName == "Two" || cardHand[2].cardName == "Two")
                {
                    if(cardHand[0].cardName == "Three" || cardHand[1].cardName == "Three" || cardHand[2].cardName == "Three")
                    {
                        idiotsArray = true;
                    }
                }
            }
            else
            {
                idiotsArray = false;
            }
        }

    }

    // here are the betting options
    public void CALLIsBetChoice()
    {
        betChoice = BettingChoice.CALL;
    }
    public void CHECKkIsBetChoice()
    {
        betChoice = BettingChoice.CHECK;
    }
    public void RAISEIsBetChoice()
    {
        betChoice = BettingChoice.RAISE;
    }
    public void ALLINIsBetChoice()
    {
        betChoice = BettingChoice.ALLIN;
    }
    public void FOLDIsBetChoice()
    {
        betChoice = BettingChoice.FOLD;
    }

    /*
     * Here are the trading options
     */
    public void ADDIsTradeChoice()
    {
        tradeChoice = TradingChoice.ADD;
    }
    public void TRADEIsTradeChoice()
    {
        tradeChoice = TradingChoice.TRADE;
    }
    public void STANDIsTradeChoice()
    {
        tradeChoice = TradingChoice.STAND;
    }
    public void ALDERAANIsTradeChoice()
    {
        tradeChoice = TradingChoice.ALDERAAN;
    }

    public void BetChips(int chipsToBet, GamePot gamePot)
    {
        // add to pot
        credits = credits - chipsToBet;
        gamePot.AddToPot(chipsToBet);
    }

    public void UpdateCurrentCardHand()
    {
        int index, valueOfCards = 0;
        Quaternion tempRotation;

        //panel = playerToUpdate.handLocation;
        //current_hand = playerToUpdate.cardHand;

        foreach (Transform child in handLocation.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        if(this.playerType == Player.PlayerType.PLAYER)
        {
            totalValue.text = "Hand Value:" + 0;
        }

        if (cardHand == null)
        {
            return;
        }

        if (cardHand.Count == 0)
        {
            return;
        }

        for (index = 0; index < cardHand.Count; index++)
        {
            valueOfCards = valueOfCards + cardHand[index].cardValue;

            cardToAdd = new GameObject();
            cardToAdd.AddComponent<CanvasRenderer>();
            cardsImage = cardToAdd.AddComponent<Image>();
            cardsImage.gameObject.name = "Card" + index;

            if (this.playerType == Player.PlayerType.PLAYER)
            {
                cardsImage.sprite = cardHand[index].cardFace;
                cardsImage.rectTransform.sizeDelta = new Vector2(250, 330);
            }
            else if (this.playerType == Player.PlayerType.AI)
            {
                if(revealCards == true)
                {
                    cardsImage.sprite = cardHand[index].cardFace;
                    cardsImage.rectTransform.sizeDelta = new Vector2(250, 330);
                }
                else
                {
                    cardsImage.sprite = cardHand[index].cardBack;
                    cardsImage.rectTransform.sizeDelta = new Vector2(50, 66);
                }
                
            }

            //cardsImage.sprite = cardHand[index].cardFace;

            //cardsImage.rectTransform.sizeDelta = new Vector2(250, 330);
            cardToAdd.GetComponent<RectTransform>().SetParent(handLocation.transform);
            cardToAdd.SetActive(true);

            if (this.playerType == Player.PlayerType.PLAYER)
            {
                cardToAdd.GetComponent<Image>().raycastTarget = true;
                cardToAdd.AddComponent<EnlargeCard>();
                cardToAdd.GetComponent<EnlargeCard>().current_index = index;
            }

            tempRotation = GetRotation(cardToAdd, index);
            cardToAdd.transform.rotation = tempRotation;
            //card_to_add.AddComponent<AllIn1Shader>();
        }

        if(this.playerType == Player.PlayerType.PLAYER)
        {
            totalValue.text = "Hand Value:" + valueOfCards.ToString();
        }
    }

    public Quaternion GetRotation(GameObject card_to_add, int index)
    {
        Vector3 returnVector;

        returnVector.x = card_to_add.transform.rotation.x;
        returnVector.y = card_to_add.transform.rotation.y;
        returnVector.z = card_to_add.transform.rotation.z;

        if (cardHand.Count >= 9)
        {
            if (index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 12;
            }
            else if (index == 1)
            {
                returnVector.z = card_to_add.transform.rotation.z + 9;
            }
            else if (index == 2)
            {
                returnVector.z = card_to_add.transform.rotation.z + 6;
            }
            else if (index == 3)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == cardHand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 12;
            }
            else if (index == cardHand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 9;
            }
            else if (index == cardHand.Count - 3)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == cardHand.Count - 4)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (cardHand.Count >= 7)
        {
            if (index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 9;
            }
            else if (index == 1)
            {
                returnVector.z = card_to_add.transform.rotation.z + 6;
            }
            else if (index == 2)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == cardHand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 9;
            }
            else if (index == cardHand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == cardHand.Count - 3)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (cardHand.Count >= 5)
        {
            if (index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 6;
            }
            else if (index == 1)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == cardHand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == cardHand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (cardHand.Count >= 3)
        {
            if (index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == cardHand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        return Quaternion.Euler(returnVector);
    }

    public void PlayVictory()
    {
        playerVocalQueues.clip = victory[Random.Range(0, victory.Count)];
        playerVocalQueues.Play();
    }

    public void PlayDefeat()
    {
        playerVocalQueues.clip = defeat[Random.Range(0, defeat.Count)];
        playerVocalQueues.Play();
    }

    public void PlayGood()
    {
        playerVocalQueues.clip = good[Random.Range(0, good.Count)];
        playerVocalQueues.Play();
    }

    public void PlayBad()
    {
        playerVocalQueues.clip = bad[Random.Range(0, bad.Count)];
        playerVocalQueues.Play();
    }

    public void GameStart()
    {
        playerVocalQueues.clip = gameStart[Random.Range(0, gameStart.Count)];
        playerVocalQueues.Play();
    }
}
