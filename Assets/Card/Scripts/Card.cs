using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer myRenderer;

    bool mouseOver = false;
    bool mouseDown = false;

    void Start()
    {
        myRenderer = GetComponent <SpriteRenderer>();
        backSprite = myRenderer.sprite;
    }

    void Update()
    {
        if (mouseOver)
        {
            
        }

        if (mouseDown)
        {
            myRenderer.sprite = faceSprite;
        }
        
    }

    void OnMouseOver()
    {
        mouseOver = true;
    }

    private void OnMouseDown()//checking procedure: to see if cards 
    {
        mouseDown = true;
    }

    /*
    void FixedUpdate()
    {
        if(GameObject.Find("Next Card"))
        {
            Debug.Log("Found");
        }
        GameObject cardRef = GameObject.Find("Game Manager");
        CardGameManager cardRefScript = cardRef.GetComponent<CardGameManager>();

        GameObject nextCardRef = GameObject.Find("nextCard");

        Debug.Log(cardRefScript.opponentHandCount);

        if (cardRefScript.opponentHand.Contains(nextCardRef))
        {
            cardRefScript.newPos = cardRefScript.opponentPos.transform.position;
            Debug.Log("True");
        }

        if (cardRefScript.playerHand.Contains(nextCardRef))
        {
            cardRefScript.newPos = cardRefScript.playerPos.transform.position;
            Debug.Log("True2");
        }

        Vector3 currentPos = Vector3.Lerp(nextCardRef.transform.position, cardRefScript.newPos, 0.10f);
        nextCardRef.transform.position = currentPos;

    }

    //Need to have a opponent deck
    */

}
