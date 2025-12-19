using UI;
using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _interactionWorldUI;

    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private Sprite avatar;
    [SerializeField] private string npcName;


    private void Awake()
    {
        if (_interactionWorldUI != null)
        {
            _interactionWorldUI.SetActive(false);
        }


    }

    public void Interact()
    {
        Debug.Log("You are interacting");

        Debug.Log($"Interacting with: {name}");
        FindFirstObjectByType<DialogueView>(FindObjectsInactive.Include).StartNewDialogue(dialogue, avatar, npcName);
    }

    public void OnInteractionAvailable()
    {
        //Debug.Log("Player near: " + gameObject.name);
        _interactionWorldUI.SetActive(true);
    
    }

    public void OnInteractionUnavailable()
    {
        //Debug.Log("Player left: " + gameObject.name);
        _interactionWorldUI.SetActive(false);
    }
}