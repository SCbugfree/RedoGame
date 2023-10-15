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

    public Vector3 hoverCurrentPos;

    public Vector3 defaultPos; //stores original y position of a Player card

    //public GameObject dmPos;
    //public DeckManager deckPos;

    SpriteRenderer inGameRenderer;

    int counter;
    int oppoIndex = 0; //the index of the card played by the Opponent in the Oppoent's deck

    bool randomPicked = false; //ensure Random selection only runs once
    bool mouseClickedCard = false; //check if mouse clicks on a Player card in Player_Turn
    bool evaluated = false;
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
    public int sortOrder = 0;
    public float timer = 1f;
    public float lerpIndex = 0.01f;
    public float returnSpd = 0.05f;

    void Start()
    {
        state = GameState.OPPO_DEAL;
        inGameRenderer = GetComponent<SpriteRenderer>();
        cardSndSource = GetComponent<AudioSource>();

    }

    void Update()
    {
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
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                    }
                    else
                    {
                        Discard();
                    }
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
         //initialization
        evaluated = false;
        mouseClickedCard = false;
        reshuffled = 0;
        randomPicked = false;
        oppoDiscard = false;
        playerDiscard = false;
        randomnizedDeck = 0;
        shuffleIndex = 0;
        animIndex = 23;
        timer = 1f;


        nextCard = DeckManager.deck[DeckManager.deck.Count - 1]; //a temporary storage of the next card dealt
        newPos = opponentPos.transform.position; //refer to the pre-determined opponent position
        newPos.x = newPos.x + (2f * opponentHand.Count); //spacing between each card


        if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
        {
            Vector3 currentPos= Vector3.Lerp(nextCard.transform.position, newPos, lerpIndex);
            nextCard.transform.position = currentPos;
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


        if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
        {
            Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, lerpIndex);
            nextCard.transform.position = currentPos;
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
                if (Vector3.Distance(oppoPlayed.transform.position, newPos) > 0.01f)
                {
                    Vector3 oppoCurrentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, lerpIndex);

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
                    if (Vector3.Distance(nextCardScript.hoverCardPos, nextCard.transform.position) < 0.01f) //if position of mouse-hover resembles nextCard
                    {
                        if (Vector3.Distance(nextCard.transform.position, previousPos[m]) < 0.01f) //card at original position
                        {
                            newPos.y = previousPos[m].y + 3; //card goes to new position when mouse hovers
                            hoverCurrentPos = Vector3.Lerp(nextCard.transform.position, newPos, lerpIndex);
                            nextCard.transform.position = hoverCurrentPos;
                        }

                        else //card already not at original position
                        {
                            if (nextCardScript.click) //if mouse is clicked when hovering above nextCard
                            {
                                if (Vector3.Distance(nextCardScript.chosenCardPos, nextCard.transform.position) < 0.01f)
                                {
                                    playerPlayed = nextCard;

                                    newPos = playerPlayedPos.transform.position;

                                    cardSndSource.PlayOneShot(dealSnd);

                                    mouseClickedCard = true;
                                }
                            }

                        }
                    }
                }
                else //mouse no longer hovers on a card
                {
                    if (Vector3.Distance(nextCard.transform.position, previousPos[m]) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, previousPos[m], lerpIndex);
                        nextCard.transform.position = currentPos;
                    }
                }
            }
        }

        else //moving the card that Player plays to predetermined position
        {
            if (Vector3.Distance(playerPlayed.transform.position, newPos) > 0.01f) //not at position
            {
                Vector3 currentPos = Vector3.Lerp(playerPlayed.transform.position, newPos, lerpIndex);

                playerPlayed.transform.position = currentPos;

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
        SpriteRenderer inGameRenderer = oppoPlayed.GetComponent<SpriteRenderer>();
        Card oppoCardScript = oppoPlayed.GetComponent<Card>();
        inGameRenderer.sprite = oppoCardScript.faceSprite;
        oppoSpr = oppoCardScript.faceSprite; //reveal Opponent face sprite

        Card playerCardScript = playerPlayed.GetComponent<Card>();
        playerSpr = playerCardScript.faceSprite;


        if (oppoSpr.name == "scissors")
        {
            if (playerSpr.name == "rock")
            {
                playerS += 1;
                playerScore.text = playerS.ToString();
                cardSndSource.PlayOneShot(winSnd);
            }
            else if (playerSpr.name == "paper")
            {
                oppoS += 1;
                oppoScore.text = oppoS.ToString();
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
                oppoS += 1;
                oppoScore.text = oppoS.ToString();
                cardSndSource.PlayOneShot(loseSnd);
            }
            else if (playerSpr.name == "paper")
            {
                playerS += 1;
                playerScore.text = playerS.ToString();
                cardSndSource.PlayOneShot(winSnd);
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
                playerS += 1;
                playerScore.text = playerS.ToString();
                cardSndSource.PlayOneShot(winSnd);
            }
            else if (playerSpr.name == "rock")
            {
                oppoS += 1;
                oppoScore.text = oppoS.ToString();
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
        newPos.y = discardPos.transform.position.y + discardPile.Count * 0.05f;
        //SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();

        if (!(oppoDiscard && playerDiscard))
        {
            if (!oppoDiscard) //discard the card Opponent plays
            {
                if (Vector3.Distance(oppoPlayed.transform.position, newPos) > 0.01f)
                {
                    Vector3 currentPos = Vector3.Lerp(oppoPlayed.transform.position, newPos, lerpIndex);
                    oppoPlayed.transform.position = currentPos;
                }
                else
                {
                    cardSndSource.PlayOneShot(dealSnd);
                    discardPile.Add(oppoPlayed);
                    oppoDiscard = true;
                    sortOrder += 1;
                }
            }
            else //discard the card Player plays
            {
                if (Vector3.Distance(playerPlayed.transform.position, newPos) > 0.01f)
                {
                    Vector3 currentPos = Vector3.Lerp(playerPlayed.transform.position, newPos, lerpIndex);
                    playerPlayed.transform.position = currentPos;
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
                Card nextCardScript = nextCard.GetComponent<Card>();

                SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();

                inGameRenderer.sprite = nextCardScript.faceSprite; //reveal Opponent Hand face sprites

                if (i < opponentHand.Count)
                {
                    nextCard = opponentHand[i];

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, lerpIndex);

                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        cardSndSource.PlayOneShot(dealSnd);
                        discardPile.Add(nextCard);
                        opponentHand.Remove(nextCard);
                    }
                }
            }

            else if ((playerHand.Count > 0) && (opponentHand.Count == 0))
            {
                if (i < playerHand.Count)
                {
                    nextCard = playerHand[i];

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, lerpIndex);

                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        cardSndSource.PlayOneShot(dealSnd);
                        discardPile.Add(nextCard);
                        playerHand.Remove(nextCard);
                    }
                }
            }
        }
    }


    void Reshuffle()
    {
        sortOrder = 0;
        SpriteRenderer inGameRenderer = nextCard.GetComponent<SpriteRenderer>();
        Card nextCardScript = nextCard.GetComponent<Card>();

        for (int t = 0; t < discardPile.Count; t++) //flip all cards (face sprite to back sprite)
        {
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

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, returnSpd);
                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        DeckManager.deck.Add(nextCard);
                        cardSndSource.PlayOneShot(dealSnd);
                        shuffleIndex++;
                    }
                }
            }
            else //all cards returned to the starting deck
            {
                if (animIndex >= 0) //play shuffling animation
                {
                    nextCard = DeckManager.deck[animIndex]; //get the card at the top

                    newPos.y = GameObject.Find("Deck Manager").transform.position.y + (DeckManager.deck.Count - animIndex) * 0.05f; //transfer it to bottom

                    if (Vector3.Distance(nextCard.transform.position, newPos) > 0.01f)
                    {
                        Vector3 currentPos = Vector3.Lerp(nextCard.transform.position, newPos, returnSpd);
                        nextCard.transform.position = currentPos;
                    }
                    else
                    {
                        animIndex--;
                        cardSndSource.PlayOneShot(dealSnd);
                    }
                }
                else //physically reshuffling the deck
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
       