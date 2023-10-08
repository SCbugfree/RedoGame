using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer myRenderer;

    public Transform playerHandPos;

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
            
        }
    }

    void OnMouseOver()
    {
        mouseOver = true;
    }

    //Need to have a opponent deck

}
