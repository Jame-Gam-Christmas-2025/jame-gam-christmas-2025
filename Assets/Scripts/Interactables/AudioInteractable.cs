using UI;
using UnityEngine;

public class AudioInteractable : Interactable
{
    private bool _isActive = false;

    public override void Interact()
    {
        base.Interact();
        
        if(_isActive)
        {
            AudioManager.Instance.PlayYuleMUS(gameObject);
        } else
        {
            AudioManager.Instance.StopYuleMUS(gameObject);
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