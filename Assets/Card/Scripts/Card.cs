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

}
