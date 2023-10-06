using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPreFab;

    public Sprite[] cardfaces; //Array of multiple elements (in this case Sprites) 

    public int deckCount; //size of the deck

    public static List<GameObject> deck = new List<GameObject>(); //List<the type>

    //public static: the whole class can access the List now, no need to GetComponent

    //example: List<int> allScores = new List<int>();

    //List and array: list take eup more memory and is more flexible,can perform more complicated things

    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            //create an instance of an object, can take serveral parameters (this case create an instance of a card)

            GameObject newCard = Instantiate(cardPreFab, gameObject.transform); //gameObject: the object on which the script attaches
            Card newCardScript = newCard.GetComponent<Card>(); //access another script
            newCardScript.faceSprite = cardfaces[i % 3]; //assign pattern of card
            deck.Add(newCard);

        }
    }




}
