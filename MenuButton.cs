using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    float transitionMultiplier;
    SoundManager soundManager;
    Arrow arrow;
    void Start()
    {
        transitionMultiplier = FindObjectOfType<BackToMenu>().transitionMultiplier;
        soundManager = FindObjectOfType<SoundManager>();
        arrow = FindObjectOfType<Arrow>();
    }

    public void OnMouseEnter()
    {
        arrow.MoveToButtonPos(Array.IndexOf(arrow.menuButtons, gameObject));
    }

    public void OnMouseDown()
    {
        ButtonPress(name);
    }

    public void ButtonPress(string buttonName)
    {
        soundManager.PlayClip("click");
        switch (buttonName)
        {
            case "Continue":
                // do something special here
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            case "New Game":
                Initiate.Fade("Game", Color.black, transitionMultiplier);
                break;
            default:
                Initiate.Fade(buttonName, Color.black, transitionMultiplier);
                break;
        }
    }
}
