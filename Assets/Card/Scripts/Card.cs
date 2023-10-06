using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer myRenderer;

    bool mouseOver = false;

    void Start()
    {
        myRenderer = GetComponent <SpriteRenderer>();
        backSprite = myRenderer.sprite;
    }

    void Update()
    {
        if (mouseOver)
        {
            myRenderer.sprite = faceSprite;
        }
    }

    void OnMouseDown() // called when user has pressed the mouse button while over the Collider.
    {
        mouseOver = true;
    }

}
