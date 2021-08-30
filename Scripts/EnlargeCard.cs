using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnlargeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject audio_source;
    GameObject game_manager;
    //RectTransform this_card;
    [SerializeField] public int current_index;
    Vector3 cached_current_scale;
    [SerializeField] public Material outline;

    // Start is called before the first frame update
    void Start()
    {
        //sibling_index = GetComponent<RectTransform>().GetSiblingIndex();
        //sibling_index = this_card.GetComponent<Transform>().GetSiblingIndex();
        cached_current_scale = transform.localScale;
        Debug.Log(transform.localScale);
        game_manager = GameObject.FindWithTag("GameController");
        audio_source = GameObject.FindWithTag("CardHover");
        outline = game_manager.GetComponent<MainGameplaySabacc>().outlineEffect;
        //outline = Resources.Load("Image", typeof(Material)) as Material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered card");
        audio_source.GetComponent<AudioSource>().Play();
        GetComponent<Image>().material = outline;
        //cards_image = card_to_add.AddComponent<Image>();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 40.0f, transform.localPosition.z);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited card");
        GetComponent<Image>().material = null;
        transform.localScale = cached_current_scale;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 40.0f, transform.localPosition.z);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer clicked card");
        //int new_index;
        if (game_manager.GetComponent<MainGameplaySabacc>().discardOn)
        {
           /* new_index = sibling_index - game_manager.GetComponent<PreSabaccGameplay>().card_hand.Count - 1;
            if( new_index < 0 )
            {
                new_index = new_index * -1;
            }
            Debug.Log("Sibling Index to discard" + sibling_index);
            Debug.Log("NEW IMPROVED INDEX" + new_index);
           */
            game_manager.GetComponent<MainGameplaySabacc>().DiscardCard(game_manager.GetComponent<MainGameplaySabacc>().players[0], current_index);
            game_manager.GetComponent<MainGameplaySabacc>().PickedCard();
        }
  
    }
}
