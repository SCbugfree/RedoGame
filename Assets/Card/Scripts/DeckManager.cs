using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameObject cardPreFab;//a class contains helper methods,or a type of object

    public Sprite[] cardfaces; //Array of multiple elements (in this case Sprites) 

    public int deckCount; //size of the deck

    public static List<GameObject> deck = new List<GameObject>(); //format: List <the type>

    //public static: the whole class can access the List now, no need to GetComponent
    //example: List<int> allScores = new List<int>();
    //List and array: list take eup more memory and is more flexible,can perform more complicated things

    void Start() //create the deck
    {
        for (int i = 0; i < deckCount; i++)
        {
            //create an instance of an object, can take serveral parameters (this case create an instance of a card)

            GameObject newCard = Instantiate(cardPreFab, gameObject.transform); //gameObject: the object on which the script attaches
            //Instantiate parameters: original, position, rotation, parent, instantiateInWorldSpace
            //Returns: the clone of original object

            Card newCardScript = newCard.GetComponent<Card>(); //access another script//??

            newCardScript.faceSprite = cardfaces[Random.Range(0,3)]; //assign pattern of card randomly

            deck.Add(newCard);

        }
    }




}
