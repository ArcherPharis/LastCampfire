using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInteract : Interactable
{
    [SerializeField] Text youWin;

    private void Start()
    {
        youWin.text = "";
    }

    public override void Interact(GameObject InteractingObject = null)
    {
        youWin.text = "You Win!";
    }
}
