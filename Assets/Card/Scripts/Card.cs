using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer myRenderer;

    public Vector3 chosenCardPos = new Vector3(0,0,0);
    public Vector3 hoverCardPos = new Vector3(0, 0, 0);

    public Vector3 targetY;

    void Start()
    {
        myRenderer = GetComponent <SpriteRenderer>();
        backSprite = myRenderer.sprite;
    }

    
    void Update()
    {
                
    }
    

    void OnMouseOver() //checking procedure: to see if mouse hovers above a card
    {
        hoverCardPos = gameObject.transform.position; //store position of the gameObject
    }

    /*
    public bool IsTouchingMouse(GameObject Card)
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Card.GetComponent<Collider2D>().OverlapPoint(point);
    }
    */

    void OnMouseDown() //checking procedure: to see if a card is clicked
    {
        chosenCardPos = gameObject.transform.position;
    }

}
