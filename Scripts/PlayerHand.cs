using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    List<Card> current_hand; // All of my cards
    [SerializeField] GameObject panel;
    [SerializeField] GameObject game_manager;
    [SerializeField] TextMeshProUGUI total_value;
    GameObject card_to_add;
    Image cards_image;
    

    public void UpdateCurrentCardHand(Player playerToUpdate)
    {
        int index, value_of_cards = 0;
        Quaternion temp_rotation;

        panel = playerToUpdate.handLocation;
        current_hand = playerToUpdate.cardHand;

        foreach (Transform child in panel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        total_value.text = "Hand Value:" + 0;

        if (current_hand == null)
        {
            return;
        }
       
        if (current_hand.Count == 0)
        {
            Debug.Log("Hand is empty");
            return;
        }

        for(index = 0; index < current_hand.Count; index++)
        {
            value_of_cards = value_of_cards + current_hand[index].ReturnCardValue();

            Debug.Log("Done some shenanigans");
            card_to_add = new GameObject();
            cards_image = card_to_add.AddComponent<Image>();
            cards_image.sprite = current_hand[index].ReturnCardFace();
            cards_image.rectTransform.sizeDelta = new Vector2(250, 330);
            card_to_add.GetComponent<RectTransform>().SetParent(panel.transform);
            card_to_add.SetActive(true);
            card_to_add.AddComponent<EnlargeCard>();
            card_to_add.GetComponent<EnlargeCard>().currentIndex = index;
            temp_rotation = GetRotation(card_to_add, index);
            card_to_add.transform.rotation = temp_rotation;
        }
        total_value.text = "Hand Value:" + value_of_cards.ToString();
    }

    public Quaternion GetRotation(GameObject card_to_add, int index)
    {
        Vector3 returnVector;

        returnVector.x = card_to_add.transform.rotation.x;
        returnVector.y = card_to_add.transform.rotation.y;
        returnVector.z = card_to_add.transform.rotation.z;
        
        if (current_hand.Count >= 9)
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
            else if (index == current_hand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 12;
            }
            else if (index == current_hand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 9;
            }
            else if (index == current_hand.Count - 3)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == current_hand.Count - 4)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (current_hand.Count >= 7)
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
            else if (index == current_hand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 9;
            }
            else if (index == current_hand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == current_hand.Count - 3)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (current_hand.Count >= 5)
        {
            if (index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 6;
            }
            else if (index == 1)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == current_hand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 6;
            }
            else if (index == current_hand.Count - 2)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        else if (current_hand.Count >= 3)
        {
            if(index == 0)
            {
                returnVector.z = card_to_add.transform.rotation.z + 3;
            }
            else if (index == current_hand.Count - 1)
            {
                returnVector.z = card_to_add.transform.rotation.z - 3;
            }
        }
        return Quaternion.Euler(returnVector);
    }
}
