using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public List<GameObject> playerHand = new List<GameObject>();//base class for all entities
    public List<GameObject> opponentHand = new List<GameObject>();

    public int playerHandCount; //keep track of how many cards the Player should have
    public Transform playerPos; //set the location of the cards

    public int opponentHandCount;
    public Transform opponentPos;

    public Transform oppoPlayedPos;
    public Transform playerPlayedPos;
    public Transform discardPos;

    public bool isOpponent; //identify whether the card is in Opponent's deck or in Player's deck
    public bool isPlayer;

    public GameObject nextCard; //the next card dealt from deck
    public GameObject oppoPlayed; //the card that Opponent plays
    public GameObject playerPlayed; //the card that Player plays

    public Card cardRef;
    public Vector3 newPos = new Vector3(0,0,0); //initializing newPos
 
    Vector3 currentPlayerPos;
    Vector3 currentOppoPos;

    SpriteRenderer inGameRenderer;

    int counter;
    int oppoIndex = 0; //the index of the card played by the Opponent in the Oppoent's deck

    bool randomPicked = false; //ensure Random selection only runs once


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

                if(opponentHand.Count < opponentHandCount)
                {
                    OppoDealCard();
                    
                }
                else
                {
                    
                    state = GameState.PLAYER_DEAL;
                }

                break;

            case GameState.PLAYER_DEAL:

                if (playerHand.Count < playerHandCount)
                {
                    PlayerDealCard();
                }
                else
                {
                    state = GameState.OPPONENT_TURN;
                }

                break;

            case GameState.OPPONENT_TURN:

                if (opponentHand.Count == opponentHandCount)
                {
                    OpponentTurn();
                }
                else
                {
                    state = GameState.PLAYER_TURN;
                }
            
                break;

            case GameState.PLAYER_TURN:

                break;

            case GameState.EVALUATION:

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

                inGameRenderer.sprite = nextCardScript.faceSprite; //reveal player card faces

                counter++;
            }
        }
        else
        {
            if (!randomPicked) //if a random opponent card has not been picked 
            {
                oppoIndex = Random.Range(0, 3); //randomly play 1 of 3 opponent card from opponent deck

                oppoPlayed = opponentHand[oppoIndex]; //stores the card that the opponent plays

                newPos = oppoPlayedPos.transform.position; //get opponent target position

                randomPicked = true;
            }

            else
            {
                if (oppoPlayed.transform.position != newPos)
                {
                    Vector3 oppoCurrentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, 0.04f);

                    oppoPlayed.transform.position = oppoCurrentPos;
                }
                else
                {
                    opponentHand.Remove(oppoPlayed);
                }
            }
        }
    }

    void PlayerTurn()
    {
        
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
