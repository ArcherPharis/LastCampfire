using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sign : Interactable //you are based off Interactable. YOU are an Interactable. The virtual.
{

    [SerializeField] Image backroundImage;
    [SerializeField] Text dialogue;
    [SerializeField] float transitionSpeed = 1f;
    [SerializeField] string[] dialogues;
    int currentDialogueIndex = 0;
    Color DialogueTextColor;
    Color DialogueBGColor;
    float Opacity;
    Coroutine TransitionCoroutine;

    void GoToNextDialogue()
    {
        if(dialogues.Length == 0)
        {
            return;
        }

        currentDialogueIndex = (currentDialogueIndex + 1) % dialogues.Length;
        dialogue.text = dialogues[currentDialogueIndex];
    }

    // basically let's you use a cutomized version of interactable without changing the original script.
    void Start()
    {
        DialogueBGColor = backroundImage.color;
        DialogueTextColor = dialogue.color;
        SetOpacity(0);

        if (dialogues.Length != 0)
        {
            dialogue.text = dialogues[0];
        }
        else
        {
            dialogue.text = "";
        }
    }

    void SetOpacity(float opacity)
    {
        opacity = Mathf.Clamp(opacity, 0, 1);
        Color ColorMult = new Color(1f, 1f, 1f, opacity);
        dialogue.color = DialogueTextColor * ColorMult;
        backroundImage.color = DialogueBGColor * ColorMult;
        Opacity = opacity;


    }

    IEnumerator TransitionOpacityTo(float newOpacity)
    {
        float Dir = newOpacity - Opacity > 0 ? 1 : -1;
        while(Opacity!=newOpacity) 
        {
            SetOpacity(Opacity + Dir * transitionSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        SetOpacity(newOpacity);
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractComponent interactableComp = other.GetComponent<InteractComponent>();
        if (interactableComp != null)
        {
            if (TransitionCoroutine != null)
            {
                StopCoroutine(TransitionCoroutine);
                TransitionCoroutine = null;

            }
            TransitionCoroutine = StartCoroutine(TransitionOpacityTo(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractComponent interactableComp = other.GetComponent<InteractComponent>();
        if (interactableComp != null)
        {
            if (TransitionCoroutine != null)
            {
                
                StopCoroutine(TransitionCoroutine);
                TransitionCoroutine = null;

            }
            
            TransitionCoroutine = StartCoroutine(TransitionOpacityTo(0f));
        }
        dialogue.text = dialogues[0]; //ontriggestay?

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact() //being treated as Interact for everything else, but overrides the original Interact and replaces with this one.
    {
        GoToNextDialogue();
    }
}
