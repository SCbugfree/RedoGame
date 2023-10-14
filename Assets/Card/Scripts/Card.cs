using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;
    public Sprite backSprite;

    SpriteRenderer myRenderer;

    public Vector3 chosenCardPos = new Vector3(0,0,0);
    public Vector3 hoverCardPos = new Vector3(0, 0, 0);

    public bool hover = false;
    public bool click = false;

    public Vector3 targetY;

    void Start()
    {
        myRenderer = GetComponent <SpriteRenderer>();
        backSprite = myRenderer.sprite;
        hover = false;
    }

    void OnMouseOver() //checking procedure: to see if mouse hovers above a card
    {
        hoverCardPos = gameObject.transform.position; //store position of the gameObject
        hover = true;
    }

    void OnMouseExit()
    {
        hover = false;
    }

    void OnMouseDown() //checking procedure: to see if a card is clicked
    {
        chosenCardPos = gameObject.transform.position;
        click = true;
        
    }

}
