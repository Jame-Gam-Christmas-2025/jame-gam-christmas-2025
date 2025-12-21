using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _interactionUI;

    [Header("Trigger")]
    [SerializeField] private Collider _interactionTrigger;

    private List<Interactable> interactablesInRange = new List<Interactable>();

    private bool _canInteract = true;

    private void Awake()
    {
        if (_interactionUI != null)
        {
            _interactionUI.SetActive(false);
        }
        
        if (_interactionTrigger == null)
        {
            Debug.LogError("InteractionTrigger not assigned in PlayerInteractor!");
        }
        else if (!_interactionTrigger.isTrigger)
        {
            Debug.LogWarning("Assigned collider is not a trigger! Set 'Is Trigger' to true.");
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!_canInteract) return;

        if (context.performed)
        {
            if (interactablesInRange.Count > 0)
            {
                Debug.Log("Interacting!");
                interactablesInRange[0].Interact();
            }
        }
    }

    public void EnableInteraction()
    {
        _canInteract = true;

        // Enable interaction UI if there are interactables around
        if (interactablesInRange.Count > 0)
        {
            _interactionUI.SetActive(true);

            // Enable every interactable
            foreach (Interactable i in interactablesInRange)
            {
                i.OnInteractionAvailable();
            }
        }
    }

    public void DisableInteraction()
    {
        _canInteract = false;

        // Disable interaction UI
        _interactionUI.SetActive(false);

        foreach (Interactable i in interactablesInRange)
        {
            i.OnInteractionUnavailable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null && !interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);

            if (_canInteract)
            {
                interactable.OnInteractionAvailable();

                _interactionUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();

        if (interactable != null && interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Remove(interactable);
            interactable.OnInteractionUnavailable();

            if (interactablesInRange.Count == 0)
            {
                _interactionUI.SetActive(false);
            }
        }
    }
}