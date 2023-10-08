using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int playerHandCount; //keep track of how many cards the player should have
    public Transform playerPos; //set the location of the cards

    public int opponentHandCount;
    public Transform opponentPos;

    public bool isOpponent; //identify whether the card is in opponent's deck or in player's deck
    public bool isPlayer;

    public Card cardRef;

    void Start()
    {
        state = GameState.OPPO_DEAL;

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
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];//a temporary storage of the next card dealt
        Vector3 newPos = opponentPos.transform.position;
        newPos.x = newPos.x + (2f * opponentHand.Count); //spacing between each card
        nextCard.transform.position = newPos;
        opponentHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);
    }

    void PlayerDealCard()
    {
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];//a temporary storage of the next card dealt
        Vector3 newPos = playerPos.transform.position;
        newPos.x = newPos.x + (2f * playerHand.Count); //spacing between each card
        nextCard.transform.position = newPos;
        playerHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);
    }
}
