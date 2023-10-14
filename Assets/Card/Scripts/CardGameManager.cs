using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    public AudioSource cardSndSource;
    public AudioClip winSnd;
    public AudioClip loseSnd;
    public AudioClip dealSnd;

    public List<GameObject> playerHand = new List<GameObject>();//base class for all entities
    public List<GameObject> opponentHand = new List<GameObject>();
    public List<GameObject> discardPile = new List<GameObject>();

    public int playerHandCount; //keep track of how many cards the Player should have
    public Transform playerPos; //set the location of the cards

    public int opponentHandCount;
    public Transform opponentPos;

    public Transform oppoPlayedPos;
    public Transform playerPlayedPos;
    public Transform discardPos;
    public Transform shufflePos;

    GameObject nextCard; //the next card dealt from deck
    GameObject oppoPlayed; //the card that Opponent plays
    GameObject playerPlayed; //the card that Player plays

    public Sprite oppoSpr;
    public Sprite playerSpr;

    public Card cardRef;
    public Vector3 newPos; //initializing newPos

    Vector3 currentPlayerPos;
    Vector3 currentOppoPos;

    public Vector3 hoverCurrentPos;

    public Vector3 defaultPos; //stores original y position of a Player card

    public GameObject dmPos;
    public DeckManager deckPos;

    SpriteRenderer inGameRenderer;

    int counter;
    int oppoIndex = 0; //the index of the card played by the Opponent in the Oppoent's deck

    bool randomPicked = false; //ensure Random selection only runs once
    bool mouseClickedCard = false; //check if mouse clicks on a Player card in Player_Turn
    bool evaluated = false;
    //bool discardPlayed = false; //check if all cards have been discarded
    int reshuffled = 0;
    bool oppoDiscard = false;
    bool playerDiscard = false;

    public List<Vector3> previousPos = new List<Vector3>(); //stpres previous positions of Player hand
    //so that they can return after mouse hovering

    public TMP_Text oppoScore;
    public TMP_Text playerScore;
    public int oppoS = 0;
    public int playerS = 0;
    public int Counter = 0;
    public int randomnizedDeck = 0;
    public int shuffleIndex = 0;
    public int animIndex = 23;


    void Start()
    {
        state = GameState.OPPO_DEAL;
        inGameRenderer = GetComponent<SpriteRenderer>();
        GameObject dmPos = GetComponent<GameObject>();
        GameObject deckPos = GetComponent<GameObject>();
        cardSndSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        //Debug.Log("reshuffled=" + reshuffled);
        //Debug.Log("DeckManager is at" + GameObject.Find("Deck Manager").transform.position);

        switch (state)
        {
            case GameState.OPPO_DEAL:

                if (opponentHand.Count < opponentHandCount) //if Opponent hand less than predetermined amount
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

                if (!evaluated)
                {
                    Evaluation();
                }
                else
                {
                    state = GameState.DISCARD;
                }
                break;

            case GameState.DISCARD:

                if ((playerHand.Count > 0) || (opponentHand.Count > 0)) //if player hand or opponent hand still has card
                {
                    Discard();
                }
                else //if all cards have been discarded
                {
                    if ((discardPile.Count == 24) && (DeckManager.deck.Count == 0)) //if all cards are in discard pile and no card in original deck
                    {
                        state = GameState.RESHUFFLE;
                    }
                    else
                    {
                        state = GameState.OPPO_DEAL;
                    }
                }
                break;

            case GameState.RESHUFFLE:

                if (reshuffled < 24)
                {
                    Reshuffle();
                }
                else
                {
                    state = GameState.OPPO_DEAL;
                }
                break;
        }
    }


    void OppoDealCard()
    {
        evaluated = false;
        mouseClickedCard = false;
        reshuffled = 0;
        randomPicked = false;
        oppoDiscard = false;
        playerDiscard = false;
        randomnizedDeck = 0;
        shuffleIndex = 0;
        animIndex = 23;


        nextCard = DeckManager.deck[DeckManager.deck.Count - 1]; //a temporary storage of the next card dealt
        newPos = opponentPos.transform.position; //refer to the pre-determined opponent position
        newPos.x = newPos.x + (2f * opponentHand.Count); //spacing between each card


        if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01)
        {
            currentOppoPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.04f);
            nextCard.transform.position = currentOppoPos;
        }
        else
        {
            cardSndSource.PlayOneShot(dealSnd);
            opponentHand.Add(nextCard);
            DeckManager.deck.Remove(nextCard);
        }
    }

    void PlayerDealCard()
    {
        nextCard = DeckManager.deck[DeckManager.deck.Count - 1];//a temporary storage of the next card dealt
        newPos = playerPos.transform.position;
        newPos.x = newPos.x + (2f * playerHand.Count); //spacing between each card


        if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01)
        {
            currentPlayerPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.04f);
            nextCard.transform.position = currentPlayerPos;
        }
        else
        {
            cardSndSource.PlayOneShot(dealSnd);
            playerHand.Add(nextCard);
            DeckManager.deck.Remove(nextCard);
        }
    }

    void OpponentTurn()
    {
        Debug.Log("oppo dealing starts");

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
                if (Vector3.Distance(oppoPlayed.transform.position, newPos) > 0.01)
                {
                    Vector3 oppoCurrentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, 0.05f);

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

                if (nextCardScript.hover) //if mouse hovers above any card
                {
                    if (Vector3.Distance(nextCardScript.hoverCardPos, nextCard.transform.position) < 0.01) //if position of mouse-hover resembles nextCard
                    {
                        if (Vector3.Distance(nextCard.transform.position, previousPos[m]) < 0.01) //card at original position
                        {
                            newPos.y = previousPos[m].y + 3; //card goes to new position when mouse hovers
                            hoverCurrentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.05f);
                            nextCard.transform.position = hoverCurrentPos;
                        }

                        else //card already not at original position
                        {
                            if (nextCardScript.click) //if mouse is clicked when hovering above nextCard
                            {
                                if (Vector3.Distance(nextCardScript.chosenCardPos, nextCard.transform.position) < 0.01)
                                {
                                    playerPlayed = nextCard;

                                    newPos = playerPlayedPos.transform.position;

                                    mouseClickedCard = true;
                                }
                            }

                        }
                    }
                }
                else //mouse no longer hovers on a card
                {
                    if (Vector3.Distance(nextCard.transform.position, previousPos[m]) > 0.01)
                    {
                        Vector3 returnCurrentPos = Vector3.Lerp(nextCard.transform.position, previousPos[m], 0.05f);
                        nextCard.transform.position = returnCurrentPos;
                    }
                }

            }
        }

        else //moving the card that Player plays to predetermined position
        {
            if (Vector3.Distance(playerPlayed.transform.position, newPos) > 0.01) //not at position
            {
                Vector3 playerCurrentPos = Vector3.Lerp(playerPlayed.transform.position, newPos, 0.05f);

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
        //Reveal Opponent face sprite
        SpriteRenderer inGameRenderer = oppoPlayed.GetComponent<SpriteRenderer>();
        Card oppoCardScript = oppoPlayed.GetComponent<Card>();
        inGameRenderer.sprite = oppoCardScript.faceSprite;
        oppoSpr = oppoCardScript.faceSprite;

        Card playerCardScript = playerPlayed.GetComponent<Card>();
        playerSpr = playerCardScript.faceSprite;


        if (oppoSpr.name == "scissors")
        {
            if (playerSpr.name == "rock")
            {
                playerScore.text = (playerS + 1).ToString();
                cardSndSource.PlayOneShot(winSnd);
            }
            else if (playerSpr.name == "paper")
            {
                oppoScore.text = (oppoS + 1).ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else
            {
                //the same sprite
            }
        }
        else if (oppoSpr.name == "rock")
        {
            if (playerSpr.name == "scissors")
            {
                oppoScore.text = (oppoS + 1).ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else if (playerSpr.name == "paper")
            {
                playerScore.text = (playerS + 1).ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else
            {
                //the same sprite
            }
        }
        else //opponent sprite: paper
        {
            if (playerSpr.name == "scissors")
            {
                playerScore.text = (playerS + 1).ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else if (playerSpr.name == "rock")
            {
                oppoScore.text = (oppoS + 1).ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else
            {
                //the same sprite
            }
        }
        evaluated = true; //end the state

    }

    void Discard()
    {
        newPos = discardPos.transform.position;

        if (!(oppoDiscard && playerDiscard))
        {
            if (!oppoDiscard)
            {
                if (Vector3.Distance(oppoPlayed.transform.position, newPos) > 0.01)
                {
                    Vector3 oppoCurrentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, 0.05f);
                    oppoPlayed.transform.position = oppoCurrentPos;
                }
                else
                {
                    cardSndSource.PlayOneShot(dealSnd);
                    discardPile.Add(oppoPlayed);
                    oppoDiscard = true;
                }
            }
            else
            {
                newPos.z = oppoPlayed.transform.position.z - 1;

                if (Vector3.Distance(playerPlayed.transform.position, newPos) > 0.01)
                {
                    Vector3 playerCurrentPos = Vector3.Lerp(playerPlayed.transform.position, newPos, 0.04f);
                    playerPlayed.transform.position = playerCurrentPos;
                }
                else
                {
                    cardSndSource.PlayOneShot(dealSnd);
                    discardPile.Add(playerPlayed);
                    playerDiscard = true;
                }
            }
        }
        else //all played cards (2) have been discarded
        {
            int i = 0;

            if ((opponentHand.Count > 0) && (playerHand.Count != 0))
            {
                SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();

                Card nextCardScript = nextCard.GetComponent<Card>();

                inGameRenderer.sprite = nextCardScript.faceSprite; //reveal Opponent Hand face sprites

                if (i < opponentHand.Count)
                {
                    nextCard = opponentHand[i];

                    newPos.z = discardPos.transform.position.z - (2 + i);

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.05f);

                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        cardSndSource.PlayOneShot(dealSnd);
                        discardPile.Add(nextCard);
                        opponentHand.Remove(nextCard);
                        i++;
                    }
                }
            }

            else if ((playerHand.Count > 0) && (opponentHand.Count == 0))
            {
                if (i < playerHand.Count)
                {
                    nextCard = playerHand[i];

                    newPos.z = discardPos.transform.position.z - (4 + i);

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.05f);

                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        cardSndSource.PlayOneShot(dealSnd);
                        discardPile.Add(nextCard);
                        playerHand.Remove(nextCard);
                        i++;
                    }
                }
            }
        }
    }

    void Reshuffle()
    {
        for (int t = 0; t < discardPile.Count; t++) //flip all cards (face sprite to back sprite)
        {
            SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();

            Card nextCardScript = nextCard.GetComponent<Card>();

            inGameRenderer.sprite = nextCardScript.backSprite; //turns to back sprite
        }

        if (reshuffled < 24)
        {
            if (shuffleIndex < 24) //animation of returning all cards to starting deck
            {
                if (DeckManager.deck.Count <= 24)
                {
                    nextCard = discardPile[shuffleIndex];

                    newPos = GameObject.Find("Deck Manager").transform.position;

                    newPos.y += shuffleIndex * 0.05f;

                    newPos.z -= shuffleIndex;

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f) //if not at original position
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.1f);
                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        DeckManager.deck.Add(nextCard);
                        shuffleIndex++;
                    }
                }
            }
            else //all cards returned to the starting deck
            {
                Debug.Log("animIndex is now " + animIndex);

                if (animIndex >= 0) //play shuffling animation
                {
                    Debug.Log("run");
                    nextCard = DeckManager.deck[DeckManager.deck.Count - 1 - animIndex];

                    newPos.y = DeckManager.deck[0].transform.position.y - animIndex * 0.05f;

                    Debug.Log("newPos.y = "+newPos.y);
            
                    newPos.z = DeckManager.deck[0].transform.position.z + animIndex;

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, 0.05f);
                        nextCard.transform.position = currentPos;
                        Debug.Log("still run");
                    }
                    else
                    {
                        animIndex--;

                        //Debug.Log("i is now " + i);
                    }
                }
                else
                {
                    discardPile.Clear();

                    int m = 0;

                    if (m < 24)
                    {
                        int j = Random.Range(m, DeckManager.deck.Count);

                        nextCard = DeckManager.deck[m];

                        DeckManager.deck[m] = DeckManager.deck[j];

                        DeckManager.deck[j] = nextCard;

                        reshuffled++;

                    }
                }
                
            }
        }
    }
}
       