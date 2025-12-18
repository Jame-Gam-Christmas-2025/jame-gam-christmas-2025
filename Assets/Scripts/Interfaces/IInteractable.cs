using UnityEngine;

public interface IInteractable
{
    
    void Interact();

    
    void OnInteractionAvailable();

    
    void OnInteractionUnavailable();
}