using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class CardGameManager : MonoBehaviour
{
    public enum GameState //a series of related constants let us use it like functions
    {
        OPPO_DEAL,
        PLAYER_DEAL,
        OPPONENT_TURN,
        PLAYER_TURN,
        EVALUATION,
        DISCARD,
        RESHUFFLE
    }


    public static GameState state;

    public List <GameObject> playerHand = new List <GameObject>();//base class for all entities
    public List <GameObject> opponentHand = new List <GameObject>();

    public int playerHandCount; //keep track of how many cards the Player should have
    public Transform playerPos; //set the location of the cards

    public int opponentHandCount;
    public Transform opponentPos;

    public Transform oppoPlayedPos;
    public Transform playerPlayedPos;
    public Transform discardPos;

    GameObject nextCard; //the next card dealt from deck
    GameObject oppoPlayed; //the card that Opponent plays
    GameObject playerPlayed; //the card that Player plays

    public Card cardRef;
    public Vector3 newPos; //initializing newPos
 
    Vector3 currentPlayerPos;
    Vector3 currentOppoPos;

    public Vector3 defaultPos; //stores original y position of a Player card

    SpriteRenderer inGameRenderer;

    int counter;
    int oppoIndex = 0; //the index of the card played by the Opponent in the Oppoent's deck

    bool randomPicked = false; //ensure Random selection only runs once
    bool mouseClickedCard = false; //check if mouse clicks on a Player card in Player_Turn

    public List <Vector3> previousPos = new List <Vector3>(); //stpres previous positions of Player hand
    //so that they can return after mouse hovering


    void Start()
    {
        state = GameState.OPPO_DEAL;
        inGameRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        switch (state)
        {
            case GameState.OPPO_DEAL:

                if(opponentHand.Count < opponentHandCount) //if Opponent hand less than predetermined amount
                {
                    OppoDealCard();
                    
                }
                else
                {
                    
                    state = GameState.PLAYER_DEAL; //Opponent hand reaches predetermined amount
                }

                break;

            case GameState.PLAYER_DEAL:

                if (playerHand.Count < playerHandCount) //if Player hand less than predetermined amount
                {
                    PlayerDealCard();
                }
                else
                {
                    state = GameState.OPPONENT_TURN; //Player hand reaches predetermined amount
                }

                break;

            case GameState.OPPONENT_TURN:

                if (opponentHand.Count == opponentHandCount) //Opponent has not played a card
                {
                    OpponentTurn();
                }
                else
                {
                    counter = 0; //initializing counter
                    state = GameState.PLAYER_TURN; //Opponent has played a card
                }
            
                break;

            case GameState.PLAYER_TURN:

                if (playerHand.Count == playerHandCount) //Player has not played a card
                {
                    PlayerTurn();
                }
                else
                {
                    state = GameState.EVALUATION; //Player has played a card
                }

                break;

            case GameState.EVALUATION:

                Debug.Log("Hi");

                break;

            case GameState.DISCARD:

                break;

            case GameState.RESHUFFLE:

                break;
        }
    }


    void OppoDealCard()
    {
        nextCard = DeckManager.deck[DeckManager.deck.Count - 1]; //a temporary storage of the next card dealt
        newPos = opponentPos.transform.position; //refer to the pre-determined opponent position
        newPos.x = newPos.x + (2f * opponentHand.Count); //spacing between each card

        
        if (nextCard.transform.position != newPos)
        {
            currentOppoPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.04f);
            nextCard.transform.position = currentOppoPos;
        }
        else
        {
            opponentHand.Add(nextCard);
            DeckManager.deck.Remove(nextCard);
        }
    }
    
    void PlayerDealCard()
    {
        nextCard = DeckManager.deck[DeckManager.deck.Count - 1];//a temporary storage of the next card dealt
        newPos = playerPos.transform.position;
        newPos.x = newPos.x + (2f * playerHand.Count); //spacing between each card


        if (nextCard.transform.position != newPos)
        {
            currentPlayerPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.04f);
            nextCard.transform.position = currentPlayerPos;
        }
        else
        {
            playerHand.Add(nextCard);
            DeckManager.deck.Remove(nextCard);
        }
    }

    void OpponentTurn()
    {
        if (counter < playerHandCount) //make sure not repeatingly revealing 3 cards of Player deck
        {

            for (int k = 0; k < playerHandCount; k++) //ensure every card in Player deck has been revealed
            {
                nextCard = playerHand[k];

                SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();

                Card nextCardScript = nextCard.GetComponent<Card>();

                inGameRenderer.sprite = nextCardScript.faceSprite; //reveal Player card faces

                counter++;
            }
        }
        else
        {
            if (!randomPicked) //if a random Opponent card has not been picked 
            {
                oppoIndex = Random.Range(0, 3); //randomly play 1 of 3 Opponent card from opponent deck

                oppoPlayed = opponentHand[oppoIndex]; //stores the card that the Opponent plays

                newPos = oppoPlayedPos.transform.position; //get Opponent target position

                randomPicked = true;
            }

            else
            {
                if (oppoPlayed.transform.position != newPos)
                {
                    Vector3 oppoCurrentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, 0.04f);

                    oppoPlayed.transform.position = oppoCurrentPos;
                }
                else //if finished, remove card from Opponent hand
                {
                    opponentHand.Remove(oppoPlayed);
                }
            }
        }
    }

    void PlayerTurn()
    {
        if (!mouseClickedCard) //if mouse does not click on a Player card
        {
            for (int m = 0; m < playerHand.Count; m++)
            {

                nextCard = playerHand[m]; //loop through Player hand

                Card nextCardScript = nextCard.GetComponent<Card>();

                previousPos.Add(nextCard.transform.position); //stores previous position of each Player card

                Debug.Log(previousPos[m]);

                Debug.Log("hover card pos"+ nextCardScript.hoverCardPos);
                Debug.Log("clicked card pos"+ nextCardScript.chosenCardPos);


                if (nextCardScript.hoverCardPos.Equals(nextCard.transform.position)) //if position of mouse-hover resembles nextCard
                {
                    Debug.Log("Hover begins");

                    newPos.y = nextCard.transform.position.y + 2; //card goes to new position when mouse hovers

                    Vector3 hoverCurrentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.04f);

                    nextCard.transform.position = hoverCurrentPos;

                    if (nextCardScript.chosenCardPos.Equals(nextCard.transform.position)) //if position of mouse-clicked-card resembles nextCard
                    {
                        Debug.Log("Card selected");

                        playerPlayed = nextCard;

                        newPos = playerPlayedPos.transform.position;

                        mouseClickedCard = true; //if mouse clicks on a Player card

                    }
                }
                else //if mouse stops hovering above this card
                {
                    Vector3 returnCurrentPos = Vector3.Lerp(nextCard.transform.position, previousPos[m], 0.04f);
                    nextCard.transform.position = returnCurrentPos;
                }
                
            }
        }
        else //moving the card that Player plays to predetermined position
        {
            if (playerPlayed.transform.position != newPos)
            {
                Vector3 playerCurrentPos = Vector3.Lerp(playerPlayed.transform.position, newPos, 0.04f);

                playerPlayed.transform.position = playerCurrentPos;
            }
            else //if finished, remove card from Player hand
            {
                playerHand.Remove(playerPlayed);
                previousPos.Clear(); //empty the list storing previous Player card positions
            }
        } 
    }

    void Evaluation()
    {

    }

    void Discard()
    {

    }

    void Reshuffle()
    {

    }

}
