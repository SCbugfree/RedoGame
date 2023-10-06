using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameManager : MonoBehaviour
{
    public enum GameState //a series of related constants let us use it like functions
    {
        DEAL,
        CHOOSE,
        RESOLVE
    }


    public static GameState state;

    public List<GameObject> playerHand = new List<GameObject>();

    public int playerHandCount; //keep track of how many cards the player should have
    public Transform playerPos; //set the location of the cards


    void Start()
    {
        state = GameState.DEAL;

    }

    void Update()
    {
        switch (state)
        {
            case GameState.DEAL:

                if (playerHand.Count < playerHandCount)
                {
                    DealCard();
                }
                else
                {
                    state = GameState.CHOOSE;
                }

                break;

            case GameState.CHOOSE:

                break;

            case GameState.RESOLVE:

                break;
        }
    }

    void DealCard()
    {
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = playerPos.transform.position;
        newPos.x = newPos.x + (2f * playerHand.Count); //spacing between each card
        nextCard.transform.position = newPos;
        playerHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);
    }
}
