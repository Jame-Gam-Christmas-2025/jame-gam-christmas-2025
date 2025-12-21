using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject interactionWorldUI;

    public virtual void Awake()
    {
        if (interactionWorldUI) interactionWorldUI.SetActive(false);
    }

    public virtual void Interact()
    {
        
    }

    public void EnableUI()
    {
        if (!interactionWorldUI) return;

        interactionWorldUI.SetActive(true);
    }

    public void DisableUI()
    {
        if (!interactionWorldUI) return;

        interactionWorldUI.SetActive(false);
    }

    public virtual void OnInteractionAvailable()
    {
        EnableUI();
    }

    public virtual void OnInteractionUnavailable()
    {
        DisableUI();
    }
}