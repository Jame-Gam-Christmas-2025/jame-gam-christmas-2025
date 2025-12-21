using UI;
using UnityEngine;

public class TestInteractable : Interactable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private Sprite avatar;
    [SerializeField] private string npcName;

    public override void Interact()
    {
        base.Interact();

        Debug.Log("You are interacting");

        Debug.Log($"Interacting with: {name}");
        DialogueView dialogueView = FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include);

        if (dialogueView != null)
        {
            dialogueView.StartNewDialogue(dialogue, avatar, npcName);
        }
    }

    public override void OnInteractionAvailable()
    {
        base.OnInteractionAvailable();

        
    }

    public override void OnInteractionUnavailable()
    {
        base.OnInteractionUnavailable();

        
    }
}