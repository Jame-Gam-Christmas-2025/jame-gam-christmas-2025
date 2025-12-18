using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _interactionUI;

    [Header("Trigger")]
    [SerializeField] private Collider _interactionTrigger;

    

    private List<IInteractable> interactablesInRange = new List<IInteractable>();

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

    public void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed && interactablesInRange.Count > 0)
        {
            interactablesInRange[0].Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null && !interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);
            interactable.OnInteractionAvailable();

            if (_interactionUI != null)
            {
                _interactionUI.SetActive(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null && interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Remove(interactable);
            interactable.OnInteractionUnavailable();

            if (interactablesInRange.Count == 0 && _interactionUI != null)
            {
                _interactionUI.SetActive(false);
                
            }
        }
    }
}