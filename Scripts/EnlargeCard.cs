using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnlargeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // variables for use throughout the code
    [SerializeField] GameObject audioSource;                                // audioSource is the cardHover sound which plays on hover
    GameObject gameManager;                                                 // GameManager is where the sabacc gameplay script is located
    [SerializeField] public int currentIndex;                               // current index is which card is being hovered on or selected
    Vector3 cachedCurrentScale;                                             // cached scale for use of returning back to the same size
    [SerializeField] public Material outline;                               // outline effect to display around card

    // Start is called before the first frame update
    void Start()
    {
        cachedCurrentScale = transform.localScale;                                      // grabs the current scale of the card
        gameManager = GameObject.FindWithTag("GameController");                         // sets the gameManager to the correct object
        audioSource = GameObject.FindWithTag("CardHover");                              // sets the audioSource to correct card sound
        outline = gameManager.GetComponent<MainGameplaySabacc>().outlineEffect;         // grabs the outline effect
    }

    /*
     * Called when pointer enters a cards outline
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        // plays the card hover sound
        audioSource.GetComponent<AudioSource>().Play();

        // applies the outline effect
        GetComponent<Image>().material = outline;
        
        // enlarges the card
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // moves the card up from current position to see it better
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 40.0f, transform.localPosition.z);
    }

    /*
     * Called when pointer exits a cards outline
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        // removes outline effect
        GetComponent<Image>().material = null;

        // returns card to 'normal'
        transform.localScale = cachedCurrentScale;

        // resets card's position
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 40.0f, transform.localPosition.z);
    }

    /*
     * Called when player clicks on a card
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        // if the player can discard a card
        if (gameManager.GetComponent<MainGameplaySabacc>().discardOn)
        {
            // discards the selected card
            gameManager.GetComponent<MainGameplaySabacc>().DiscardCard(gameManager.GetComponent<MainGameplaySabacc>().players[0], currentIndex);
            gameManager.GetComponent<MainGameplaySabacc>().PickedCard();
        }
  
    }
}
