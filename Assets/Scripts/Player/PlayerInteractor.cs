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
        if (!_canInteract)
        {
            Debug.Log("Deny");
            return;
        } 

        if (context.performed)
        {
            // Vérifiez combien d'objets interactables sont dans la portée
            int interactableCount = interactablesInRange.Count;

            if (interactableCount > 0)
            {
                GameObject firstInteractable = interactablesInRange[0].gameObject;

                // Log des informations détaillées
                Debug.Log($"Interacting with: {firstInteractable.name}");
                Debug.Log($"Total interactables in range: {interactableCount}");

                // Si l'objet a une interaction spécifique, loggez cela aussi
                if (interactablesInRange[0] is Interactable interactable)
                {
                    Debug.Log($"Interactable Type: {interactable.GetType().Name}");
                }
                else
                {
                    Debug.Log("The first object is not an interactable.");
                }

                // Exécutez l'interaction
                interactablesInRange[0].Interact();
            }
            else
            {
                Debug.Log("No interactable objects in range.");
            }
        }

    }

    public void EnableInteraction()
    {
        _canInteract = true;


        // Enable interaction UI if there are interactables around
        if (interactablesInRange.Count > 0)
        {
            interactablesInRange = new List<Interactable>();
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
        Interactable interactable = other.GetComponentInParent<Interactable>();

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
        Interactable interactable = other.GetComponentInParent<Interactable>();

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